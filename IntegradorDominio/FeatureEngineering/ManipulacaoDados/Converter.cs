using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.ManipulacaoDados
{
    [Feature("Converter", "Manipulação de Dados")]
    [FeatureName("Converter")]
    public class Converter : IFeature
    {
        public string NomeExibicao => "Converter";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col { get; set; }
        public string type { get; set; }
    }
}
