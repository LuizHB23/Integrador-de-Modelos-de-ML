using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.JanelasTemporais
{
    [FeatureName("Expanding")]
    public class Expanding : IFeature
    {
        public string NomeExibicao => "Expanding";

        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
