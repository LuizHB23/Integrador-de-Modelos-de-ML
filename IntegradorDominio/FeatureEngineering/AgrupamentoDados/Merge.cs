using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;

namespace IntegradorDominio.FeatureEngineering.AgrupamentoDados
{
    [Feature("Merge", "Agroupamento de Dados")]
    [FeatureName("Merge")]
    public class Merge : IFeature
    {
        public string NomeExibicao => "Merge";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string right {  get; set; }
        public string on { get; set; }
    }
}
