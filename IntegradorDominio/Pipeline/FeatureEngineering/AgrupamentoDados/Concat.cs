using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.AgrupamentoDados
{
    public class Concat : IStepFeature
    {
        public string NomeExibicao => "Concat";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
