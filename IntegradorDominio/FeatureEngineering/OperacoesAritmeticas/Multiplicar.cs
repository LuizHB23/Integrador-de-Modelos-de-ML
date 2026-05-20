using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;

namespace IntegradorDominio.FeatureEngineering.OperacoesAritmeticas
{
    [Feature("Mult", "Operações Aritméticas")]
    [FeatureName("Mult")]
    public class Multiplicar : IFeature
    {
        public string NomeExibicao => "Multiplicar";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string left { get; set; }
        public string right { get; set; }
        public string value { get; set; }
        public string exit { get; set; }
    }
}
