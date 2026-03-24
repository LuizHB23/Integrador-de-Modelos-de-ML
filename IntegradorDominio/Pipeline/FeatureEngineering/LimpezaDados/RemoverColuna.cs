using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.LimpezaDados
{
    public class RemoverColuna : IStepFeature
    {
        public string NomeExibicao => "Remover Coluna";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
