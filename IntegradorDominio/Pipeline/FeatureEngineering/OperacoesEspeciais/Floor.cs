using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesEspeciais
{
    public class Floor : IStepFeature
    {
        public string NomeExibicao => "Floor";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
