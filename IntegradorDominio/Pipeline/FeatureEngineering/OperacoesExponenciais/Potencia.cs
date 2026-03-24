using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesExponenciais
{
    public class Potencia : IStepFeature
    {
        public string NomeExibicao => "Potência";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
