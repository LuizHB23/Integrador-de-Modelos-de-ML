using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEstatisticas
{
    public class Moda : IFeature
    {
        public string NomeExibicao => "Moda";
        public string NomeCodigo => throw new NotImplementedException();
    }
}
