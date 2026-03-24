using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesEspeciais
{
    public class Minimo : IStepFeature
    {
        public string NomeExibicao => "Mínimo";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
