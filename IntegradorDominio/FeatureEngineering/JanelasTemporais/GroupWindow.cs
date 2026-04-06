using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.JanelasTemporais
{
    [FeatureName("GroupWindow")]
    public class GroupWindow : IFeature
    {
        public string NomeExibicao => "GroupWindow";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col { get; set; }
        public string agg { get; set; }
        public string exit { get; set; }
    }
}
