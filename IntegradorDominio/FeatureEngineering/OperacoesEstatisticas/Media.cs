using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;

namespace IntegradorDominio.FeatureEngineering.OperacoesEstatisticas
{
    [Feature("Mean", "Operações Estatísticas")]
    [FeatureName("Mean")]
    public class Media : IFeature
    {
        public string NomeExibicao => "Média";
        public Dictionary<string, object?>? Contexto { get; set; }
        
        public string col { get; set; }
    }
}
