using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesAritmeticas
{
    public class Subtrair : IStepFeature
    {
        public string NomeExibicao => "Subtrair";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
