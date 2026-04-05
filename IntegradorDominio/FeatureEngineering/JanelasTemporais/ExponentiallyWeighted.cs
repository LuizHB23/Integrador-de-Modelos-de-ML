using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.JanelasTemporais
{
    [FeatureName("ExponentiallyWeighted")]
    public class ExponentiallyWeighted : IFeature
    {
        public string NomeExibicao => "ExponentiallyWeighted";

        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
