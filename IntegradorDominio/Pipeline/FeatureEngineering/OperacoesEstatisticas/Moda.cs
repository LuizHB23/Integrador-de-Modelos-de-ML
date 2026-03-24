using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesEstatisticas
{
    public class Moda : IStepFeature
    {
        public string NomeExibicao => "Moda";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
