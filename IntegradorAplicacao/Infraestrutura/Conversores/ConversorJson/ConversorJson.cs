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
                [typeof(ModeloConfiguracao)] = new ModeloJson(provider),
                [typeof(Dictionary<int,SchemaDTO>)] = new CardsJson<SchemaDTO>(provider),
                [typeof(Dictionary<int, FuncaoDTO>)] = new CardsJson<FuncaoDTO>(provider),
                [typeof(Dictionary<int, TransformadorDTO>)] = new CardsJson<TransformadorDTO>(provider),
                [typeof(Dictionary<int, SaidaDTO>)] = new CardsJson<SaidaDTO>(provider)
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
