using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;

namespace IntegradorDominio.FeatureEngineering.LimpezaDados
{
    [FeatureName("FillNa")]
    public class FillNa : IFeature
    {
        public string NomeExibicao => "Fill NA";

        public string col { get; set; }
        public string value {  get; set; }
    }
}
