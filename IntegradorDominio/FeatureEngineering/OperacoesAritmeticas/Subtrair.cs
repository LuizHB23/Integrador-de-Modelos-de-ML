using IntegradorDominio.Attributes;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.InterfacesSteps;

namespace IntegradorDominio.FeatureEngineering.OperacoesAritmeticas
{
    [FeatureName("Subtrair")]
    [FeatureName("Sub")]
    public class Subtrair : IFeature
    {
        public string NomeExibicao => "Subtrair";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string exit { get; set; }
        public string left { get; set; }
        public string right { get; set; }

        public Subtrair() { }
        public Subtrair(string nomeColunaSaida, string colunaEsquerda, string colunaDireita)
        {
            exit = nomeColunaSaida;
            left = colunaEsquerda;
            right = colunaDireita;
        }
    }
}
