using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEspeciais
{
    public class Floor : IFeature
    {
        public string NomeExibicao => "Floor";
        public string NomeCodigo => throw new NotImplementedException();
    }
}
