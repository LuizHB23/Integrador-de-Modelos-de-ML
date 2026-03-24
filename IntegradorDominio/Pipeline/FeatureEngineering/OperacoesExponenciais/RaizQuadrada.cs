using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesExponenciais
{
    public class RaizQuadrada : IStepFeature
    {
        public string NomeExibicao => "Raiz Quadrada";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
