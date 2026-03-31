using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.AgrupamentoDados
{
    [FeatureName("Sort")]
    public class Sort : IFeature
    {
        public string NomeExibicao => "Sort";
        public Dictionary<string, object?>? Contexto { get ; set; }

        public string col { get; set; }
        public string asc { get; set; }
    }
}
