using IntegradorAplicacao.DTO;
using IntegradorDominio.Models.DataFrameModel;
using IntegradorDominio.Models.Inferencia;
using IntegradorDominio.Models.ModeloEtapas;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace IntegradorAplicacao.Aplicacao.InferenciaAplicacao
{
    public class ConfiguraInputsOutputs
    {
        public List<ErrosInferencia> ListaErros { get; private set; }

        public ConfiguraInputsOutputs(List<ErrosInferencia> listaErros)
        {
            ListaErros = listaErros;
        }

        public List<NamedOnnxValue> CriarInputs(
            DataFrame df,
            InferenceSession session,
            Dictionary<int, Schema>? schemaDicionario,
            string[]? ids)
        {
            var inputs = new List<NamedOnnxValue>();

            var inputName = session.InputMetadata.Keys.First();

            var colunasFeature = df.Colunas
                .Where(c => !DeveIgnorar(schemaDicionario, c.Nome))
                .ToList();

            int linhas = df.QuantidadeLinhas;
            int features = colunasFeature.Count;

            var dados = new List<float>();

            int linhasValidas = 0;

            for (int i = 0; i < linhas; i++)
            {
                bool erro = false;
                var linhaTemp = new List<float>();

                try
                {
                    foreach (var col in colunasFeature)
                    {
                        linhaTemp.Add(
                            ConverterParaFloat(col.PegarValor(i)));
                    }
                }
                catch (Exception ex)
                {
                    erro = true;

                    ErrosInferencia linha = new()
                    {
                        IndexLinha = i,
                        Id = ids[i],
                        Erro = ex.Message,
                        Outputs = new()
                    };

                    foreach (var col in colunasFeature)
                        linha.Outputs.Add(col.Nome, col.PegarValor(i));

                    ListaErros.Add(linha);
                }

                if (!erro)
                {
                    dados.AddRange(linhaTemp);
                    linhasValidas++;
                }
            }

            var tensor = new DenseTensor<float>(dados.ToArray(), new[] { linhasValidas, features });

            inputs.Add(
                NamedOnnxValue.CreateFromTensor(
                    inputName,
                    tensor));

            return inputs;
        }

        public List<NamedOnnxValue> AjustarInputsParaModelo(
            List<NamedOnnxValue> inputs,
            InferenceSession session)
        {
            var nomesEsperados = session.InputMetadata.Keys.ToList();

            var novosInputs = new List<NamedOnnxValue>();

            for (int i = 0; i < nomesEsperados.Count; i++)
            {
                var nomeEsperado = nomesEsperados[i];

                var tensor = inputs[i].AsTensor<float>();

                if (tensor == null)
                    throw new Exception(
                        $"Tensor nulo no input: {nomeEsperado}");

                // 🔥 MATERIALIZA
                var dados = tensor.ToArray();
                var dims = tensor.Dimensions.ToArray();

                var novoTensor = new DenseTensor<float>(
                    dados,
                    dims);

                novosInputs.Add(
                    NamedOnnxValue.CreateFromTensor(
                        nomeEsperado,
                        novoTensor));
            }

            return novosInputs;
        }

        // 🔥 CORREÇÃO PRINCIPAL
        public List<NamedOnnxValue> ConverterParaInputs(
            IDisposableReadOnlyCollection<DisposableNamedOnnxValue> resultados)
        {
            var inputs = new List<NamedOnnxValue>();

            foreach (var r in resultados)
            {
                var tensor = r.AsTensor<float>();

                if (tensor == null)
                    throw new Exception($"Tensor nulo: {r.Name}");

                // 🔥 COPIA REAL DOS DADOS
                var dados = tensor.ToArray();

                // 🔥 COPIA REAL DAS DIMENSÕES
                var dims = tensor.Dimensions.ToArray();

                // 🔥 NOVO TENSOR GERENCIADO
                var novoTensor = new DenseTensor<float>(
                    dados,
                    dims);

                inputs.Add(
                    NamedOnnxValue.CreateFromTensor(
                        r.Name,
                        novoTensor));
            }

            return inputs;
        }

        private bool DeveIgnorar(
            Dictionary<int, Schema>? schemaDicionario,
            string nomeColuna)
        {
            var valor = schemaDicionario.Values
                .FirstOrDefault(c => c.NomeColuna == nomeColuna);

            if (valor is null)
                throw new Exception(
                    $"Coluna não tratada: {nomeColuna}");

            return valor.Finalidade != "Feature";
        }

        private float ConverterParaFloat(object valor)
        {
            if (valor is float f) return f;

            if (valor is double d)
            {
                var valorFloat = (float)d;

                if (float.IsNaN(valorFloat) ||
                    float.IsInfinity(valorFloat))
                {
                    throw new Exception(
                        $"Overflow ao converter double: {d}");
                }

                return valorFloat;
            }

            if (valor is int i) return i;

            if (valor == null)
                throw new Exception("Linha com valor vazio");

            if (float.TryParse(valor.ToString(), out var result))
            {
                if (float.IsNaN(result) ||
                    float.IsInfinity(result))
                {
                    throw new Exception(
                        $"Valor inválido: {result}");
                }

                return result;
            }

            throw new Exception($"Valor inválido: {valor}");
        }

        public List<ResultadoInferencia> ReconstruirSaidaComId(
            IDisposableReadOnlyCollection<DisposableNamedOnnxValue> resultados,
            string[] ids)
        {
            var resultado = new List<ResultadoInferencia>();

            var indicesComErro = new HashSet<int>(
                ListaErros.Select(e => e.IndexLinha));

            var indicesValidos = new List<int>();

            for (int i = 0; i < ids.Length; i++)
            {
                if (!indicesComErro.Contains(i))
                {
                    indicesValidos.Add(i);

                    resultado.Add(new ResultadoInferencia
                    {
                        Id = ids[i],
                        Outputs = new Dictionary<string, float[]>()
                    });
                }
            }

            foreach (var r in resultados)
            {
                float[] valores;
                int[] dims;

                if (r.Value is DenseTensor<float> tf)
                {
                    valores = tf.ToArray();
                    dims = tf.Dimensions.ToArray();
                }
                else if (r.Value is DenseTensor<long> tl)
                {
                    valores = tl.Select(x => (float)x).ToArray();
                    dims = tl.Dimensions.ToArray();
                }
                else if (r.Value is DenseTensor<int> ti)
                {
                    valores = ti.Select(x => (float)x).ToArray();
                    dims = ti.Dimensions.ToArray();
                }
                else if (r.Value is DenseTensor<double> td)
                {
                    valores = td.Select(x => (float)x).ToArray();
                    dims = td.Dimensions.ToArray();
                }
                else if (r.Value is DenseTensor<bool> tb)
                {
                    valores = tb.Select(x => x ? 1f : 0f).ToArray();
                    dims = tb.Dimensions.ToArray();
                }
                else
                {
                    throw new Exception(
                        $"Tipo não suportado: {r.Value.GetType()}");
                }

                int tamanhoPorLinha = dims.Length == 1
                    ? 1
                    : dims
                        .Skip(1)
                        .Aggregate(1, (a, b) => a * b);

                for (int j = 0; j < indicesValidos.Count; j++)
                {
                    var slice = new float[tamanhoPorLinha];

                    // 🔥 BUG CORRIGIDO
                    Array.Copy(
                        valores,
                        j * tamanhoPorLinha,
                        slice,
                        0,
                        tamanhoPorLinha
                    );

                    resultado[j].Outputs[r.Name] = slice;
                }
            }

            return resultado;
        }
    }
}