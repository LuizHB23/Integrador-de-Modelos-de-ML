using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesExponenciais
{
    public class Exponencial : IStepFeature
    {
        public string NomeExibicao => "Exponencial";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
