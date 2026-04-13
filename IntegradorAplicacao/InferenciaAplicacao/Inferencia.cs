using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.PipelineAplicacao.ExecutorAplicacao;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.Inferencia;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Diagnostics;

namespace IntegradorAplicacao.InferenciaAplicacao
{
    public class Inferencia
    {
        private readonly IConverteJson<Dictionary<int, TransformadorDTO>> _conversorTransformadores;
        private readonly IConverteJson<Dictionary<int, FuncaoDTO>> _conversorPipeline;
        private readonly IConverteJson<Dictionary<int, SchemaDTO>> _conversorSchema;

        private Dictionary<int, SchemaDTO>? _schemaDicionario;
        private readonly ExecutorFinal _executor;

        public Inferencia(IConverteJson<Dictionary<int, FuncaoDTO>> conversorPipeline, IConverteJson<Dictionary<int, SchemaDTO>> conversorSchema, IConverteJson<Dictionary<int, TransformadorDTO>> conversorTransformadores)
        {
            _conversorTransformadores = conversorTransformadores;
            _conversorPipeline = conversorPipeline;
            _conversorSchema = conversorSchema;

            _executor = new ExecutorFinal(_conversorPipeline);
        }


        public async Task<List<ResultadoInferencia>> RealizaInferenciaAsync(DataFrame dataFrame, string caminhoModelo, string caminhoSchema, string caminhoPipeline, string caminhoTransformadores)
        {
            _schemaDicionario = _conversorSchema.CarregarJson(caminhoSchema)
                ?? throw new Exception("Schema não carregado.");

            var dataFrameNovo = await RealizaFeatureEngineeringAsync(dataFrame, caminhoPipeline);

            var transformadores = _conversorTransformadores.CarregarJson(caminhoTransformadores);

            var idSchema = _schemaDicionario.FirstOrDefault(s => s.Value.Finalidade == "ID");

            if (idSchema.Value is null)
                throw new Exception("Nenhuma coluna com Finalidade == ID foi definida no schema.");

            var nomeColunaId = idSchema.Value.NomeColuna;

            var colunaId = dataFrameNovo.PegarColunaBase(nomeColunaId);

            if (colunaId is null)
                throw new Exception($"Coluna ID '{nomeColunaId}' não encontrada no DataFrame.");

            var quantidade = colunaId.Quantidade;

            var ids = new string[quantidade];

            for (int i = 0; i < quantidade; i++)
            {
                ids[i] = colunaId.PegarValor(i)?.ToString();
            }

            List<NamedOnnxValue> inputs;

            if (transformadores.Count > 0)
            {
                using (var primeiraSession = new InferenceSession(transformadores.First().Value.CaminhoTransformador))
                {
                    inputs = CriarInputs(dataFrameNovo, primeiraSession);
                }

                IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? resultados = null;

                foreach (var transformador in transformadores.OrderBy(t => t.Key))
                {
                    using var session = new InferenceSession(transformador.Value.CaminhoTransformador);

                    var inputsAjustados = AjustarInputsParaModelo(inputs, session);

                    resultados = session.Run(inputsAjustados);

                    DebugSaida(resultados, $"Transformador {transformador.Key}");

                    inputs = ConverterParaInputs(resultados);
                }

                using var finalSession = new InferenceSession(caminhoModelo);

                var finalInputs = AjustarInputsParaModelo(inputs, finalSession);

                var finalResultados = finalSession.Run(finalInputs);

                DebugSaida(finalResultados, "Modelo Final");

                return ReconstruirSaidaComId(finalResultados, ids);
            }
            else
            {
                using var session = new InferenceSession(caminhoModelo);

                inputs = CriarInputs(dataFrameNovo, session);

                var resultados = session.Run(inputs);

                DebugSaida(resultados, "Modelo Final");

                return ReconstruirSaidaComId(resultados, ids);
            }
        }

        private async Task<DataFrame> RealizaFeatureEngineeringAsync(DataFrame dataFrame, string caminhoPipeline)
        {
            await Task.Run(() => _executor.ConstroiSequenciaMetodoPipeline(caminhoPipeline));
            return await Task.Run(() => _executor.ExecutarTudo(dataFrame));
        }

        private List<NamedOnnxValue> CriarInputs(DataFrame df, InferenceSession session)
        {
            var inputs = new List<NamedOnnxValue>();

            var inputName = session.InputMetadata.Keys.First();

            var colunasFeature = df.Colunas
                .Where(c => !DeveIgnorar(c.Nome))
                .ToList();

            int linhas = df.QuantidadeLinhas;
            int features = colunasFeature.Count;

            var dados = new float[linhas * features];

            int index = 0;

            for (int i = 0; i < linhas; i++)
            {
                foreach (var col in colunasFeature)
                {
                    dados[index++] = ConverterParaFloat(col.PegarValor(i));
                }
            }

            var tensor = new DenseTensor<float>(dados, new[] { linhas, features });

            inputs.Add(NamedOnnxValue.CreateFromTensor(inputName, tensor));

            return inputs;
        }

        private List<NamedOnnxValue> AjustarInputsParaModelo(List<NamedOnnxValue> inputs, InferenceSession session)
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

        private List<NamedOnnxValue> ConverterParaInputs(IDisposableReadOnlyCollection<DisposableNamedOnnxValue> resultados)
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

        private bool DeveIgnorar(string nomeColuna)
        {
            var valor = _schemaDicionario
                .FirstOrDefault(c => c.Value.NomeColuna == nomeColuna);

            if (valor.Value is null)
                throw new Exception($"Coluna não tratada: {nomeColuna}");

            return valor.Value.Finalidade != "Feature";
        }

        private float ConverterParaFloat(object valor)
        {
            if (valor == null) return 0f;
            if (valor is float f) return f;
            if (valor is double d) return (float)d;
            if (valor is int i) return i;

            if (float.TryParse(valor.ToString(), out var result))
                return result;

            throw new Exception($"Valor inválido: {valor}");
        }

        private List<ResultadoInferencia> ReconstruirSaidaComId(IDisposableReadOnlyCollection<DisposableNamedOnnxValue> resultados, string[] ids)
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

        private void DebugSaida(IDisposableReadOnlyCollection<DisposableNamedOnnxValue> resultados, string etapa) 
        { 
            Debug.WriteLine($"===== SAÍDA: {etapa} ====="); 
            foreach (var r in resultados)
            {
                var nome = r.Name; var tipo = r.Value.GetType(); 
                Debug.WriteLine($"Output: {nome}"); 
                Debug.WriteLine($"Tipo: {tipo}"); 

                if (r.Value is DenseTensor<float> tf)
                {
                    var valores = tf.ToArray();
                    Debug.WriteLine($"Valores (float): {string.Join(", ", valores.Take(100))}"); 
                } 
                else if (r.Value is DenseTensor<long> tl)
                { 
                    var valores = tl.ToArray(); Debug.WriteLine($"Valores (long): {string.Join(", ", valores.Take(20))}"); 
                } 
                else
                {
                    Debug.WriteLine("Tipo não suportado para debug.");
                }
                Debug.WriteLine("----------------------------"); 
            } 
        }
    }
}