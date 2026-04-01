using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;

namespace IntegradorDominio.FeatureEngineering.AgrupamentoDados
{
    [FeatureName("Merge")]
    public class Merge : IFeature
    {
        public string NomeExibicao => "Merge";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string right {  get; set; }
        public string on { get; set; }
    }
}
