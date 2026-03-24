using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.FeatureEngineering.OperacoesAritmeticas
{
    public class Mod : IStepFeature
    {
        public string NomeExibicao => "Mod";

        public void ExecutarExpression()
        {
            throw new NotImplementedException();
        }
    }
}
