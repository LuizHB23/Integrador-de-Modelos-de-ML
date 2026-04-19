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
        private readonly ConfiguraInputsOutputs _configuracao;

        public List<List<object?>> ListaErros { get; private set; }

        public Inferencia(IConverteJson<Dictionary<int, FuncaoDTO>> conversorPipeline, IConverteJson<Dictionary<int, SchemaDTO>> conversorSchema, IConverteJson<Dictionary<int, TransformadorDTO>> conversorTransformadores)
        {
            _conversorTransformadores = conversorTransformadores;
            _conversorPipeline = conversorPipeline;
            _conversorSchema = conversorSchema;

            ListaErros = new();

            _executor = new(_conversorPipeline);
            _configuracao = new(_schemaDicionario, ListaErros);
        }

        public async Task<List<ResultadoInferencia>> RealizaInferenciaAsync(DataFrame dataFrame, string caminhoModelo, string caminhoSchema, string caminhoPipeline, string caminhoTransformadores)
        {
            _schemaDicionario = _conversorSchema.CarregarJson(caminhoSchema)
                ?? throw new Exception("Schema não carregado.");

            var dataFrameNovo = await RealizaFeatureEngineeringAsync(dataFrame, caminhoPipeline);

            var transformadores = _conversorTransformadores.CarregarJson(caminhoTransformadores);

            var ids = PegaIds(dataFrameNovo);

            List<NamedOnnxValue> inputs;

            if (transformadores.Count > 0)
            {
                using (var primeiraSession = new InferenceSession(transformadores.First().Value.CaminhoTransformador))
                {
                    inputs = _configuracao.CriarInputs(dataFrameNovo, primeiraSession);
                }

                IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? resultados = null;

                foreach (var transformador in transformadores.OrderBy(t => t.Key))
                {
                    resultados = RealizaInferenciaOnnx(inputs, transformador.Value.CaminhoTransformador);

                    DebugSaida(resultados, $"Transformador {transformador.Key}");

                    inputs = _configuracao.ConverterParaInputs(resultados);
                }

                var finalResultados = RealizaInferenciaOnnx(inputs, caminhoModelo);

                DebugSaida(finalResultados, "Modelo Final");

                return _configuracao.ReconstruirSaidaComId(finalResultados, ids);
            }
            else
            {
                var resultados = RealizaInferenciaOnnx(dataFrameNovo, caminhoModelo);

                DebugSaida(resultados, "Modelo Final");

                return _configuracao.ReconstruirSaidaComId(resultados, ids);
            }
        }

        private async Task<DataFrame> RealizaFeatureEngineeringAsync(DataFrame dataFrame, string caminhoPipeline)
        {
            await Task.Run(() => _executor.ConstroiSequenciaMetodoPipeline(caminhoPipeline));
            return await Task.Run(() => _executor.ExecutarTudo(dataFrame));
        }

        private IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? RealizaInferenciaOnnx(object inputs, string caminho)
        {
            using var session = new InferenceSession(caminho);

            List<NamedOnnxValue>? inputsFinais = null;

            if (inputs is DataFrame dataFrame)
            {
                inputsFinais = _configuracao.CriarInputs(dataFrame, session);
            }
            else if (inputs is List<NamedOnnxValue> listaInputs)
            {
                inputsFinais = _configuracao.AjustarInputsParaModelo(listaInputs, session);
            }
            else
            {
                throw new Exception("Erro ao ajustar onnx");
            }


            return session.Run(inputsFinais);
        }

        private string[] PegaIds(DataFrame dataFrame)
        {
            var idSchema = _schemaDicionario.FirstOrDefault(s => s.Value.Finalidade == "ID");

            if (idSchema.Value is null)
                throw new Exception("Nenhuma coluna com Finalidade == ID foi definida no schema.");

            var nomeColunaId = idSchema.Value.NomeColuna;

            var colunaId = dataFrame.PegarColunaBase(nomeColunaId);

            if (colunaId is null)
                throw new Exception($"Coluna ID '{nomeColunaId}' não encontrada no DataFrame.");

            var quantidade = colunaId.Quantidade;

            var ids = new string[quantidade];

            for (int i = 0; i < quantidade; i++)
            {
                ids[i] = colunaId.PegarValor(i)?.ToString();
            }

            return ids;
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