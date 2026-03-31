using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.AgrupamentoDados
{
    public class Merge : IFeature
    {
        public string NomeExibicao => "Merge";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
