using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEstatisticas
{
    public class Percentil : IFeature
    {
        public string NomeExibicao => "Percentil";
        public string NomeCodigo => throw new NotImplementedException();
    }
}
