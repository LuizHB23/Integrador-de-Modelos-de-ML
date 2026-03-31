using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.AgrupamentoDados
{
    public class OrderBy : IFeature
    {
        public string NomeExibicao => "OrderBy";
        public Dictionary<string, object?>? Contexto { get ; set; }
    }
}
