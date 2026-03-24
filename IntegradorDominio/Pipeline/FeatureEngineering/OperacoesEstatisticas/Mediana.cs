using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesEstatisticas
{
    public class Mediana : IStepFeature
    {
        public string NomeExibicao => "Mediana";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
