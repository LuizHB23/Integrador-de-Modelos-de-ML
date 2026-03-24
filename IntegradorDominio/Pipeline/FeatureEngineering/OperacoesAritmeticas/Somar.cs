using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesAritmeticas
{
    public class Somar : IStepFeature
    {
        public string NomeExibicao => "Somar";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
