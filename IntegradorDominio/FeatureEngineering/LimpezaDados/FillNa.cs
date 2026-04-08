using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;

namespace IntegradorDominio.FeatureEngineering.LimpezaDados
{
    [Feature("FillNa", "Limpeza de Dados")]
    [FeatureName("FillNa")]
    public class FillNa : IFeature
    {
        public string NomeExibicao => "Fill NA";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col { get; set; }
        public string value {  get; set; }
    }
}
