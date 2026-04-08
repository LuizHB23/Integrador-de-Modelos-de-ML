using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;

namespace IntegradorDominio.FeatureEngineering.MapearLinhas
{
    [Feature("Map", "Mapear Linhas")]
    [FeatureName("Map")]
    public class Map : IFeature
    {
        public string NomeExibicao => "Map";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string lambdax { get; set; }
    }
}
