using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.AgrupamentoDados
{
    public class Pivot : IStepFeature
    {
        public string NomeExibicao => "Pivot";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
