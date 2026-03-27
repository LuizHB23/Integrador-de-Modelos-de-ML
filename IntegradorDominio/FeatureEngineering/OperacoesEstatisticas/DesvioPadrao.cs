using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEstatisticas
{
    public class DesvioPadrao : IFeature
    {
        public string NomeExibicao => "Desvio Padrão";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
