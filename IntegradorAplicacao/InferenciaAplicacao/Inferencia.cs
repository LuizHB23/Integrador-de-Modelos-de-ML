using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.PipelineAplicacao.ExecutorAplicacao;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.Inferencia;
using Microsoft.ML.OnnxRuntime;

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
            _configuracao = new(ListaErros);
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
                    inputs = _configuracao.CriarInputs(dataFrameNovo, primeiraSession, _schemaDicionario);
                }

                IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? resultados = null;

                foreach (var transformador in transformadores.OrderBy(t => t.Key))
                {
                    resultados = RealizaInferenciaOnnx(inputs, transformador.Value.CaminhoTransformador);

                    inputs = _configuracao.ConverterParaInputs(resultados);
                }

                var finalResultados = RealizaInferenciaOnnx(inputs, caminhoModelo);

                return _configuracao.ReconstruirSaidaComId(finalResultados, ids);
            }
            else
            {
                var resultados = RealizaInferenciaOnnx(dataFrameNovo, caminhoModelo);

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
                inputsFinais = _configuracao.CriarInputs(dataFrame, session, _schemaDicionario);
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
    }
}