using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.AgrupamentoDados
{
    public class Pivot : IFeature
    {
        public string NomeExibicao => "Pivot";
        public string NomeCodigo => throw new NotImplementedException();
    }
}
