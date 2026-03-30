using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.LimpezaDados
{
    [FeatureName("Converter")]
    [FeatureName("Convert")]
    public class Converter : IFeature
    {
        public string NomeExibicao => "Converter";

        public string col { get; set; }
        public string type { get; set; }
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
