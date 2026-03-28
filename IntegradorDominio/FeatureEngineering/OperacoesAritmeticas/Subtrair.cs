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
        public string NomeCodigo => "Sub";

        public string exit { get; set; }
        public Coluna<float> left;
        public Coluna<float> right;

        public Subtrair() { }
        public Subtrair(string nomeColunaSaida, Coluna<float> colunaEsquerda, Coluna<float> colunaDireita)
        {
            exit = nomeColunaSaida;
            left = colunaEsquerda;
            right = colunaDireita;
        }
    }
}
