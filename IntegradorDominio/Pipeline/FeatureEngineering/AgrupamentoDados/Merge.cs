using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.AgrupamentoDados
{
    public class Merge : IStepFeature
    {
        public string NomeExibicao => "Merge";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
