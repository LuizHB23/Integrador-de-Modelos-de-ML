using IntegradorDominio.Attributes;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.InterfacesSteps;

namespace IntegradorDominio.FeatureEngineering.OperacoesAritmeticas
{
    [Feature("Sub", "Operações Aritméticas")]
    [FeatureName("Sub")]
    public class Subtrair : IFeature
    {
        public string NomeExibicao => "Subtrair";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string left { get; set; }
        public string right { get; set; }
        public string value { get; set; }
        public string exit { get; set; }
    }
}
