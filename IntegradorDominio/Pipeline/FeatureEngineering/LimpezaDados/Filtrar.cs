using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.LimpezaDados
{
    public class Filtrar : IStepFeature
    {
        public string NomeExibicao => "Filtrar";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
