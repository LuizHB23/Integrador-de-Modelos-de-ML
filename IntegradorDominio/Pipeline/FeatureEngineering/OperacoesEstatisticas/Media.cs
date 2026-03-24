using IntegradorDominio.Pipeline.InterfacesSteps;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesEstatisticas
{
    public class Media : IStepFeature
    {
        public string NomeExibicao => "Média";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
