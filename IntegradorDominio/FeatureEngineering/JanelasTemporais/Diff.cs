using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.JanelasTemporais
{
    [FeatureName("Diff")]
    public class Diff : IFeature
    {
        public string NomeExibicao => "Diff";

        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
