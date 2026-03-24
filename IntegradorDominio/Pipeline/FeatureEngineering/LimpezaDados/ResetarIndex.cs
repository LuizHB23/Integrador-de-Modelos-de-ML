using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.LimpezaDados
{
    public class ResetarIndex : IStepFeature
    {
        public string NomeExibicao => "Resetar Index";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
