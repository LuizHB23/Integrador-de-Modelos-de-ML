using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.LimpezaDados
{
    public class FillNa : IStepFeature
    {
        public string NomeExibicao => "Fill NA";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
