using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson.ConverteJson;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorDominio.Models.Configuracao;
using IntegradorAplicacao.DTO;

namespace IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson
{
    public class ConversorJson : IConversorJson
    {
        private readonly Dictionary<Type, object> _conversores;
        private readonly IPathProvider _provider;

        public ConversorJson(IPathProvider provider)
        {
            _provider = provider;

            _conversores = new Dictionary<Type, object>
            {
                [typeof(ModeloEmUsoConfiguracao)] = new ModeloEmUsoJson(),

                [typeof(Dictionary<int, SchemaDTO>)] = new CardsJson<SchemaDTO>(),

                [typeof(Dictionary<int, FuncaoDTO>)] = new CardsJson<FuncaoDTO>(),

                [typeof(Dictionary<int, SaidaDTO>)] = new CardsJson<SaidaDTO>(),

                [typeof(SchemaConfiguracao)] = new TempJson<SchemaConfiguracao>(),

                [typeof(PipelineTratamentoConfiguracao)] = new TempJson<PipelineTratamentoConfiguracao>(),

                [typeof(TransformadorConfiguracao)] = new TempJson<TransformadorConfiguracao>(),

                //[typeof(Dictionary<int, TransformadorDTO>)] = new CardsJson<TransformadorDTO>(),

                [typeof(List<ModeloConfiguracao>)] = new ConfiguradoresJson<ModeloConfiguracao>(),

                [typeof(List<SchemaConfiguracao>)] = new ConfiguradoresJson<SchemaConfiguracao>(),

                [typeof(List<PipelineTratamentoConfiguracao>)] = new ConfiguradoresJson<PipelineTratamentoConfiguracao>(),

                [typeof(List<PipelineSaidaInferenciaConfiguracao>)] = new ConfiguradoresJson<PipelineSaidaInferenciaConfiguracao>(),

                [typeof(List<TransformadorConfiguracao>)] = new ConfiguradoresJson<TransformadorConfiguracao>()
            };
        }

        public async Task<T> CarregarJsonAsync<T>(string nomeModelo) where T : class
        {
            IConverteJson<T> conversor = (IConverteJson<T>)_conversores[typeof(T)];
            return await conversor.CarregarJsonAsync(PegaJson<T>(nomeModelo));
        }

        public async Task ConverteJsonAsync<T>(T objeto, string nomeModelo) where T : class
        {
            IConverteJson<T> conversor = (IConverteJson<T>)_conversores[typeof(T)];
            await conversor.ConverteJsonAsync(objeto, PegaJson<T>(nomeModelo));
        }

        private string PegaJson<T>(string nomeModelo)
        {
            return typeof(T) switch
            {
                Type tipo when tipo == typeof(ModeloEmUsoConfiguracao) => _provider.GetCaminhoModeloEmUsoConfig(nomeModelo),

                Type tipo when tipo == typeof(Dictionary<int, SchemaDTO>) => nomeModelo,

                Type tipo when tipo == typeof(Dictionary<int, FuncaoDTO>) => nomeModelo,

                Type tipo when tipo == typeof(Dictionary<int, SaidaDTO>) => nomeModelo,

                Type tipo when tipo == typeof(SchemaConfiguracao) => _provider.GetCaminhoTempConfig(nomeModelo),

                Type tipo when tipo == typeof(PipelineTratamentoConfiguracao) => _provider.GetCaminhoTempConfig(nomeModelo),

                Type tipo when tipo == typeof(TransformadorConfiguracao) => _provider.GetCaminhoTempConfig(nomeModelo),

                Type tipo when tipo == typeof(List<ModeloConfiguracao>) => _provider.GetCaminhoModeloConfig(nomeModelo),

                Type tipo when tipo == typeof(List<SchemaConfiguracao>) => _provider.GetCaminhoSchemaConfig(nomeModelo),

                Type tipo when tipo == typeof(List<PipelineTratamentoConfiguracao>) => _provider.GetCaminhoPipelineConfig(nomeModelo),

                Type tipo when tipo == typeof(List<TransformadorConfiguracao>) => _provider.GetCaminhoTransformadorConfig(nomeModelo),

                Type tipo when tipo == typeof(List<PipelineSaidaInferenciaConfiguracao>) => _provider.GetCaminhoSaidaConfig(nomeModelo),

                _ => throw new Exception("Esse tipo não existe")
            };
        }
    }
}
