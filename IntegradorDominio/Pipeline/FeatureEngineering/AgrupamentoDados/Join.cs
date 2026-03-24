using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.AgrupamentoDados
{
    public class Join : IStepFeature
    {
        public string NomeExibicao => "Join";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
