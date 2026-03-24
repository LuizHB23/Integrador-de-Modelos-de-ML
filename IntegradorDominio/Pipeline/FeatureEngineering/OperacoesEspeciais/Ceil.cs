using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesEspeciais
{
    public class Ceil : IStepFeature
    {
        public string NomeExibicao => "Ceil";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
