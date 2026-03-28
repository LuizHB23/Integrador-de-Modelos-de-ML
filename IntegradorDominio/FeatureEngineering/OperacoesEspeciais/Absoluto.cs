using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEspeciais
{
    public class Absoluto : IFeature
    {
        public string NomeExibicao => "Absoluto";

        public string NomeCodigo => throw new NotImplementedException();

    }
}
