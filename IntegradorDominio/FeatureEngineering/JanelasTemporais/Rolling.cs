using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.JanelasTemporais
{
    [FeatureName("Rolling")]
    public class Rolling : IFeature
    {
        public string NomeExibicao => "Rolling";

        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
