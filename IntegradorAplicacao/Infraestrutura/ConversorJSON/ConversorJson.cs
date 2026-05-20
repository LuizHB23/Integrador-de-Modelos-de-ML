using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.ConversorJson.Conversores;
using IntegradorDominio.Models.Configuracao;

namespace IntegradorAplicacao.Infraestrutura.ConversorJson
{
    public class ConversorJson 
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

        public T CarregarJson<T>(string caminho) where T : class
        {
            IConverteJson<T> conversor = (IConverteJson<T>)_conversores[typeof(T)];
            return conversor.CarregarJson(caminho);
        }

        public void ConverteJson<T>(T objeto) where T : class
        {
            IConverteJson<T> conversor = (IConverteJson<T>)_conversores[typeof(T)];
            conversor.ConverteJson(objeto);
        }
    }
}
