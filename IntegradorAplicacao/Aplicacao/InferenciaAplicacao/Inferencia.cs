using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorAplicacao;
using IntegradorDominio.Models.Configuracao;
using IntegradorDominio.Models.Configuracao.Interfaces;
using IntegradorDominio.Models.DataFrameModel;
using IntegradorDominio.Models.Inferencia;
using IntegradorDominio.Models.ModeloEtapas;
using Microsoft.ML.OnnxRuntime;
using System.Diagnostics;

namespace IntegradorAplicacao.Aplicacao.InferenciaAplicacao
{
    public class Inferencia<T> where T : IPipelineConfiguracao
    {
        private Dictionary<int, Schema>? _schemaDicionario;
        private readonly ConfiguraInputsOutputs _configuracao;
        private ExecutorFinal<T>? _executor;

        public List<ErrosInferencia> ListaErros { get; private set; }
        public HistoricoInferencia Historico { get; private set; }

        public Inferencia()
        {
            ListaErros = new();
            Historico = new();

            _executor = new();
            _configuracao = new(ListaErros);
        }

        public async Task<List<ResultadoInferencia>> RealizaInferenciaAsync(DataFrame dataFrame, SchemaConfiguracao schema, T? pipeline, TransformadorConfiguracao? transformadores, string caminhoModelo)
        {
            _schemaDicionario = schema.Dicionario;

            var dataFrameNovo = await RealizaFeatureEngineeringAsync(dataFrame, pipeline);
            _executor = null;

            var ids = PegaIds(dataFrameNovo);

            List<NamedOnnxValue> inputs;

            if(transformadores is not null)
            {
                if (transformadores.Dicionario.Count > 0)
                {
                    using (var primeiraSession = new InferenceSession(transformadores.Dicionario.First().Value.CaminhoTransformador))
                    {
                        inputs = _configuracao.CriarInputs(dataFrameNovo, primeiraSession, _schemaDicionario, ids);
                    }

                    IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? resultadosTranformadores = null;

                    foreach (var transformador in transformadores.Dicionario.OrderBy(t => t.Key))
                    {
                        resultadosTranformadores = RealizaInferenciaOnnx(inputs, transformador.Value.CaminhoTransformador, ids);

                        inputs = _configuracao.ConverterParaInputs(resultadosTranformadores);
                    }

                    var finalResultados = RealizaInferenciaOnnx(inputs, caminhoModelo, ids);

                    GeraHistorico(finalResultados);

                    return _configuracao.ReconstruirSaidaComId(finalResultados, ids);
                }

            }
            
            var resultados = RealizaInferenciaOnnx(dataFrameNovo, caminhoModelo, ids);

            GeraHistorico(resultados);

            return _configuracao.ReconstruirSaidaComId(resultados, ids);
        }

        private async Task<DataFrame> RealizaFeatureEngineeringAsync(DataFrame dataFrame, T pipeline)
        {
            await Task.Run(() => _executor.ConstroiSequenciaMetodoPipeline(pipeline));
            return await Task.Run(() => _executor.ExecutarTudo(dataFrame));
        }

        private IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? RealizaInferenciaOnnx(object inputs, string caminho, string[]?ids)
        {
            using var session = new InferenceSession(caminho);

            List<NamedOnnxValue>? inputsFinais = null;

            if (inputs is DataFrame dataFrame)
            {
                inputsFinais = _configuracao.CriarInputs(dataFrame, session, _schemaDicionario, ids);
            }
            else if (inputs is List<NamedOnnxValue> listaInputs)
            {
                inputsFinais = _configuracao.AjustarInputsParaModelo(listaInputs, session);
            }
            else
            {
                throw new Exception("Erro ao ajustar onnx");
            }

            Debug.WriteLine("=================================");

            foreach (var input in session.InputMetadata)
            {
                Debug.WriteLine($"Input: {input.Key}");
                Debug.WriteLine($"Dimensões: {string.Join(",", input.Value.Dimensions)}");
            }

            Debug.WriteLine("=================================");

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

        private void GeraHistorico(IDisposableReadOnlyCollection<DisposableNamedOnnxValue> resultados)
        {
            Historico.TotalLinhas =  resultados.Count + ListaErros.Count;
            Historico.LinhasComErro = ListaErros.Count;

            if (ListaErros.Count > 0)
            {
                Historico.Status = "Parcial";
            }
            else
            {
                Historico.Status = "Sucesso";
            }
        }
    }
}