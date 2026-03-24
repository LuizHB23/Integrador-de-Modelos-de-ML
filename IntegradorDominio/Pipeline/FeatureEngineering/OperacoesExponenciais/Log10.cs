using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesExponenciais
{
    public class Log10 : IStepFeature
    {
        public string NomeExibicao => "Log10";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
