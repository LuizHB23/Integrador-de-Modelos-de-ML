using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.AgrupamentoDados
{
    public class OrderBy : IStepFeature
    {
        public string NomeExibicao => "OrderBy";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
