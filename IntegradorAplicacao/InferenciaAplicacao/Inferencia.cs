using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.PipelineAplicacao.ExecutorAplicacao;
using IntegradorDominio.DataFrameModel;
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

        public Inferencia(
            IConverteJson<Dictionary<int, FuncaoDTO>> conversorPipeline,
            IConverteJson<Dictionary<int, SchemaDTO>> conversorSchema,
            IConverteJson<Dictionary<int, TransformadorDTO>> conversorTransformadores)
        {
            _conversorTransformadores = conversorTransformadores;
            _conversorPipeline = conversorPipeline;
            _conversorSchema = conversorSchema;

            _executor = new ExecutorFinal(_conversorPipeline);
        }

        public Dictionary<string, float[]> RealizaInferencia(
            DataFrame dataFrame,
            string caminhoModelo,
            string caminhoSchema,
            string caminhoPipeline,
            string caminhoTransformadores)
        {
            _schemaDicionario = _conversorSchema.CarregarJson(caminhoSchema)
                ?? throw new Exception("Schema não carregado.");

            var dataFrameNovo = RealizaFeatureEngineering(dataFrame, caminhoPipeline);
            var transformadores = _conversorTransformadores.CarregarJson(caminhoTransformadores);

            if (transformadores.Count > 0)
            {
                List<NamedOnnxValue> inputs;

                // 🔹 Primeiro ONNX usa DataFrame
                using (var primeiraSession = new InferenceSession(transformadores.First().Value.CaminhoTransformador))
                {
                    inputs = CriarInputs(dataFrameNovo, primeiraSession);
                }

                IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? resultados = null;

                // 🔁 Pipeline ONNX encadeado
                foreach (var transformador in transformadores.OrderBy(t => t.Key))
                {
                    using var session = new InferenceSession(transformador.Value.CaminhoTransformador);

                    var inputsAjustados = AjustarInputsParaModelo(inputs, session);

                    resultados = session.Run(inputsAjustados);

                    DebugSaida(resultados, $"Transformador {transformador.Key}");

                    inputs = ConverterParaInputs(resultados);
                }

                // 🎯 Modelo final
                using var finalSession = new InferenceSession(caminhoModelo);

                var finalInputs = AjustarInputsParaModelo(inputs, finalSession);

                var finalResultados = finalSession.Run(finalInputs);

                DebugSaida(finalResultados, "Modelo Final");

                return ConverterSaida(finalResultados);
            }
            else
            {
                using var session = new InferenceSession(caminhoModelo);

                var inputs = CriarInputs(dataFrameNovo, session);

                var resultados = session.Run(inputs);

                DebugSaida(resultados, "Modelo Final");

                return ConverterSaida(resultados);
            }
        }

        private DataFrame RealizaFeatureEngineering(DataFrame dataFrame, string caminhoPipeline)
        {
            _executor.ConstroiSequenciaMetodoPipeline(caminhoPipeline);
            return _executor.ExecutarTudo(dataFrame);
        }

        // =========================
        // 🔧 CORE
        // =========================

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

        private List<NamedOnnxValue> AjustarInputsParaModelo(
            List<NamedOnnxValue> inputs,
            InferenceSession session)
        {
            var nomesEsperados = session.InputMetadata.Keys.ToList();

            if (inputs.Count < nomesEsperados.Count)
                throw new Exception("Quantidade de inputs menor que o esperado.");

            var novosInputs = new List<NamedOnnxValue>();

            for (int i = 0; i < nomesEsperados.Count; i++)
            {
                var nomeEsperado = nomesEsperados[i];
                var tensor = inputs[i].AsTensor<float>();

                novosInputs.Add(NamedOnnxValue.CreateFromTensor(nomeEsperado, tensor));
            }

            return novosInputs;
        }

        private List<NamedOnnxValue> ConverterParaInputs(
            IDisposableReadOnlyCollection<DisposableNamedOnnxValue> resultados)
        {
            var inputs = new List<NamedOnnxValue>();

            foreach (var r in resultados)
            {
                var tensor = r.AsTensor<float>();
                inputs.Add(NamedOnnxValue.CreateFromTensor(r.Name, tensor));
            }

            return inputs;
        }

        private Dictionary<string, float[]> ConverterSaida(
            IDisposableReadOnlyCollection<DisposableNamedOnnxValue> resultados)
        {
            var output = new Dictionary<string, float[]>();

            foreach (var r in resultados)
            {
                var nome = r.Name;

                if (r.Value is DenseTensor<float> tf)
                {
                    output[nome] = tf.ToArray();
                }
                else if (r.Value is DenseTensor<long> tl)
                {
                    // 🔥 conversão de Int64 → float
                    output[nome] = tl.Select(x => (float)x).ToArray();
                }
                else
                {
                    throw new Exception($"Tipo não suportado: {r.Value.GetType()}");
                }
            }

            return output;
        }

        // =========================
        // 🔌 AUXILIARES
        // =========================

        private bool DeveIgnorar(string nomeColuna)
        {
            if (_schemaDicionario == null)
                throw new Exception("Schema não carregado.");

            var valor = _schemaDicionario
                .FirstOrDefault(c => c.Value.NomeColuna == nomeColuna);

            if (valor.Value is null)
                throw new Exception($"Coluna não tratada: {nomeColuna}");

            return valor.Value.Finalidade != "Feature";
        }

        private float ConverterParaFloat(object valor)
        {
            if (valor == null)
                return 0f;

            if (valor is float f) return f;
            if (valor is double d) return (float)d;
            if (valor is int i) return i;

            if (float.TryParse(valor.ToString(), out var result))
                return result;

            throw new Exception($"Valor inválido para conversão: {valor}");
        }

        private void DebugSaida(IDisposableReadOnlyCollection<DisposableNamedOnnxValue> resultados, string etapa)
        {
            Debug.WriteLine($"===== SAÍDA: {etapa} =====");

            foreach (var r in resultados)
            {
                var nome = r.Name;
                var tipo = r.Value.GetType();

                Debug.WriteLine($"Output: {nome}");
                Debug.WriteLine($"Tipo: {tipo}");

                if (r.Value is DenseTensor<float> tf)
                {
                    var valores = tf.ToArray();
                    Debug.WriteLine($"Valores (float): {string.Join(", ", valores.Take(5))}...");
                }
                else if (r.Value is DenseTensor<long> tl)
                {
                    var valores = tl.ToArray();
                    Debug.WriteLine($"Valores (long): {string.Join(", ", valores.Take(5))}...");
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