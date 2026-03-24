using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.LimpezaDados
{
    public class RenomearColuna : IStepFeature
    {
        public string NomeExibicao => "Renomear Coluna";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
