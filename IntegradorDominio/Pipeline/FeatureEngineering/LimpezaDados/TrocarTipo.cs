using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.LimpezaDados
{
    public class TrocarTipo : IStepFeature
    {
        public string NomeExibicao => "Trocar Tipo";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
