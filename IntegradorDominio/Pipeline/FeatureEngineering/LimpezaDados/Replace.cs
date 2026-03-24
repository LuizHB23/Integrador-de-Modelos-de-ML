using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.LimpezaDados
{
    public class Replace : IStepFeature
    {
        public string NomeExibicao => "Replace";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
