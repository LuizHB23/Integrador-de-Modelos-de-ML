using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesEspeciais
{
    public class Maximo : IStepFeature
    {
        public string NomeExibicao => "Máximo";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
