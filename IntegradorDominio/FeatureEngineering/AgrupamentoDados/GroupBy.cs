using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.AgrupamentoDados
{
    [FeatureName("GroupBy")]
    public class GroupBy : IFeature
    {
        public string NomeExibicao => "GroupBy";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col { get; set; }
        public string agg { get; set; }
    }
}
