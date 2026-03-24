using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.LimpezaDados
{
    public class RemoverDuplicados : IStepFeature
    {
        public string NomeExibicao => "Remover Duplicados";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
