using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;

namespace IntegradorDominio.FeatureEngineering.OperacoesAritmeticas
{
    [Feature("Div", "Operações Aritméticas")]
    [FeatureName("Div")]
    public class Dividir : IFeature
    {
        public string NomeExibicao => "Dividir";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string left { get; set; }
        public string right { get; set; }
        public string value { get; set; }
        public string exit { get; set; }
    }
}
