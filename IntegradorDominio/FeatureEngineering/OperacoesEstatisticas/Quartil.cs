using IntegradorDominio.InterfacesSteps;

namespace IntegradorDominio.FeatureEngineering.OperacoesEstatisticas
{
    public class Quartil : IFeature
    {
        public string NomeExibicao => "Quartil";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
