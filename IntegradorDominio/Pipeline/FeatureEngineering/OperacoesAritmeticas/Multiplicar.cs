using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesAritmeticas
{
    public class Multiplicar : IStepFeature
    {
        public string NomeExibicao => "Multiplicar";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
