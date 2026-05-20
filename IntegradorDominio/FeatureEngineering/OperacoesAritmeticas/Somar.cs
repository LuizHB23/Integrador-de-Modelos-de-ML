using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;

namespace IntegradorDominio.FeatureEngineering.OperacoesAritmeticas
{
    [Feature("Sum", "Operações Aritméticas")]
    [FeatureName("Sum")]
    public class Somar : IFeature
    {
        public string NomeExibicao => "Somar";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string left { get; set; }
        public string right { get; set; }
        public string value { get; set; }
        public string exit { get; set; }
    }
}
