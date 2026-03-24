using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesEstatisticas
{
    public class Percentil : IStepFeature
    {
        public string NomeExibicao => "Percentil";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
