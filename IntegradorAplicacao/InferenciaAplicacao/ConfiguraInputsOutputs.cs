using IntegradorAplicacao.DTO;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.Inferencia;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace IntegradorAplicacao.InferenciaAplicacao
{
    public class ConfiguraInputsOutputs
    {
        public List<List<object?>> ListaErros { get; private set; }

        public ConfiguraInputsOutputs(List<List<object?>> listaErros)
        {
            ListaErros = listaErros;
        }

        public List<NamedOnnxValue> CriarInputs(DataFrame df, InferenceSession session, Dictionary<int, SchemaDTO>? schemaDicionario)
        {
            var inputs = new List<NamedOnnxValue>();

            var inputName = session.InputMetadata.Keys.First();

            var colunasFeature = df.Colunas
                .Where(c => !DeveIgnorar(schemaDicionario, c.Nome))
                .ToList();

            int linhas = df.QuantidadeLinhas;
            int features = colunasFeature.Count;

            var dados = new float[linhas * features];

            int index = 0;

            for (int i = 0; i < linhas; i++)
            {
                int startIndex = index;
                bool erro = false;

                try
                {
                    foreach (var col in colunasFeature)
                    {
                        dados[index++] = ConverterParaFloat(col.PegarValor(i));
                    }
                }
                catch (Exception ex)
                {
                    erro = true;

                    List<object?> linha = new();
                    foreach (var col in colunasFeature)
                        linha.Add(col.PegarValor(i));

                    linha.Add(ex.Message);
                    ListaErros.Add(linha);
                }

                if (erro)
                {
                    index = startIndex;
                }
            }

            var tensor = new DenseTensor<float>(dados, new[] { linhas, features });

            inputs.Add(NamedOnnxValue.CreateFromTensor(inputName, tensor));

            return inputs;
        }

        public List<NamedOnnxValue> AjustarInputsParaModelo(List<NamedOnnxValue> inputs, InferenceSession session)
        {
            var nomesEsperados = session.InputMetadata.Keys.ToList();

            var novosInputs = new List<NamedOnnxValue>();

            for (int i = 0; i < nomesEsperados.Count; i++)
            {
                var nomeEsperado = nomesEsperados[i];
                var tensor = inputs[i].AsTensor<float>();

                novosInputs.Add(NamedOnnxValue.CreateFromTensor(nomeEsperado, tensor));
            }

            return novosInputs;
        }

        public List<NamedOnnxValue> ConverterParaInputs(IDisposableReadOnlyCollection<DisposableNamedOnnxValue> resultados)
        {
            var inputs = new List<NamedOnnxValue>();

            foreach (var r in resultados)
            {
                var tensor = r.AsTensor<float>();
                inputs.Add(NamedOnnxValue.CreateFromTensor(r.Name, tensor));
            }

            return inputs;
        }

        private Dictionary<string, float[]> ConverterSaida(IDisposableReadOnlyCollection<DisposableNamedOnnxValue> resultados)
        {
            var output = new Dictionary<string, float[]>();

            foreach (var r in resultados)
            {
                if (r.Value is DenseTensor<float> tf)
                    output[r.Name] = tf.ToArray();

                else if (r.Value is DenseTensor<long> tl)
                    output[r.Name] = tl.Select(x => (float)x).ToArray();

                else
                    throw new Exception($"Tipo não suportado: {r.Value.GetType()}");
            }

            return output;
        }

        private bool DeveIgnorar(Dictionary<int, SchemaDTO>? schemaDicionario, string nomeColuna)
        {
            var valor = schemaDicionario.Values.FirstOrDefault(c => c.NomeColuna == nomeColuna);

            if (valor is null)
                throw new Exception($"Coluna não tratada: {nomeColuna}");

            return valor.Finalidade != "Feature";
        }

        private float ConverterParaFloat(object valor)
        {
            if (valor is float f) return f;

            if (valor is double d)
            {
                var valorFloat = (float)d;

                if (float.IsNaN(valorFloat) || float.IsInfinity(valorFloat))
                    throw new Exception($"Overflow ao converter double: {d}");

                return valorFloat;

            }

            if (valor is int i) return i;

            if (valor == null) throw new Exception("Linha com valor vazio");

            if (float.TryParse(valor.ToString(), out var result))
            {
                if (float.IsNaN(result) || float.IsInfinity(result))
                {
                    throw new Exception($"Valor inválido: {result}");
                }

                return result;
            }

            throw new Exception($"Valor inválido: {valor}");
        }

        public List<ResultadoInferencia> ReconstruirSaidaComId(IDisposableReadOnlyCollection<DisposableNamedOnnxValue> resultados, string[] ids)
        {
            var outputs = ConverterSaida(resultados);

            var resultado = new List<ResultadoInferencia>();

            int linhas = ids.Length;

            foreach (var output in outputs)
            {
                var valores = output.Value;

                int tamanhoPorLinha = valores.Length / linhas;

                for (int i = 0; i < linhas; i++)
                {
                    if (resultado.Count <= i)
                    {
                        resultado.Add(new ResultadoInferencia
                        {
                            Id = ids[i],
                            Outputs = new Dictionary<string, float[]>()
                        });
                    }

                    var slice = new float[tamanhoPorLinha];

                    Array.Copy(
                        valores,
                        i * tamanhoPorLinha,
                        slice,
                        0,
                        tamanhoPorLinha
                    );

                    resultado[i].Outputs[output.Key] = slice;
                }
            }

            return resultado;
        }
    }
}