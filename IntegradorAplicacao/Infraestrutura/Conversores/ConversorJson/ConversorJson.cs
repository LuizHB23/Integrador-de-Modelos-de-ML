using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson.ConverteJson;
using IntegradorDominio.Models.Configuracao;

namespace IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson
{
    public class ConversorJson : IConversorJson
    {
        private readonly Dictionary<Type, object> _conversores;

        public ConversorJson(IPathProvider provider)
        {
            _conversores = new Dictionary<Type, object>
            {
                [typeof(ModeloEmUsoConfiguracao)] = new ModeloEmUsoJson(provider),
                [typeof(Dictionary<int,SchemaDTO>)] = new ConfiguradoresJson<SchemaDTO>(provider),
                [typeof(Dictionary<int, FuncaoDTO>)] = new ConfiguradoresJson<FuncaoDTO>(provider),
                [typeof(Dictionary<int, TransformadorDTO>)] = new ConfiguradoresJson<TransformadorDTO>(provider),
                [typeof(Dictionary<int, SaidaDTO>)] = new ConfiguradoresJson<SaidaDTO>(provider)
            };
        }

        public async Task<T> CarregarJsonAsync<T>(string caminho) where T : class
        {
            IConverteJson<T> conversor = (IConverteJson<T>)_conversores[typeof(T)];
            return await conversor.CarregarJsonAsync(caminho);
        }

        public async Task ConverteJsonAsync<T>(T objeto) where T : class
        {
            IConverteJson<T> conversor = (IConverteJson<T>)_conversores[typeof(T)];
            await conversor.ConverteJsonAsync(objeto);
        }
    }
}
