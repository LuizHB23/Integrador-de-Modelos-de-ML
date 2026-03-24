using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesEstatisticas
{
    public class Variancia : IStepFeature
    {
        public string NomeExibicao => "Variância";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
