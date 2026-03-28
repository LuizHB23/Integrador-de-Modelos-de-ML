using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.LimpezaDados
{
    public class Filtrar : IFeature
    {
        public string NomeExibicao => "Filtrar";
        public string NomeCodigo => throw new NotImplementedException();
    }
}
