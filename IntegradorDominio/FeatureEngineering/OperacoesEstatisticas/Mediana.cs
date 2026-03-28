using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEstatisticas
{
    public class Mediana : IFeature
    {
        public string NomeExibicao => "Mediana";
        public string NomeCodigo => throw new NotImplementedException();
    }
}
