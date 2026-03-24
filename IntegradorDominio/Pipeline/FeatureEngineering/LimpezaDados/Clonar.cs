using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.LimpezaDados
{
    public class Clonar : IStepFeature
    {
        public string NomeExibicao => "Clonar";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
