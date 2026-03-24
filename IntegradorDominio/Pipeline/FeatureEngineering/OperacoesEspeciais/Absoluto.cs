using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesEspeciais
{
    public class Absoluto : IStepFeature
    {
        public string NomeExibicao => "Absoluto";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
