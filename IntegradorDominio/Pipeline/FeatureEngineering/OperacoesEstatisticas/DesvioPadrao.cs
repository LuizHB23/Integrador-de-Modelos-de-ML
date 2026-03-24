using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesEstatisticas
{
    public class DesvioPadrao : IStepFeature
    {
        public string NomeExibicao => "Desvio Padrão";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
