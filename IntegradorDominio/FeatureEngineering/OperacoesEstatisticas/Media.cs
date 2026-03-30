using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;

namespace IntegradorDominio.FeatureEngineering.OperacoesEstatisticas
{
    [FeatureName("Mean")]
    public class Media : IFeature
    {
        public string NomeExibicao => "Média";
        
        public string col { get; set; }
    }
}
