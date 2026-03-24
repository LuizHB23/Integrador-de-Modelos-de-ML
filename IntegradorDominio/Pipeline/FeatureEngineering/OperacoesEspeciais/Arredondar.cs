using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesEspeciais
{
    public class Arredondar : IStepFeature
    {
        public string NomeExibicao => "Arredondar";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
