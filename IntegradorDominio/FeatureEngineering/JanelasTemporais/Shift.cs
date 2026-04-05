using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.JanelasTemporais
{
    [FeatureName("Shift")]
    public class Shift : IFeature
    {
        public string NomeExibicao => "Shift";

        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
