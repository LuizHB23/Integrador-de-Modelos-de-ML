using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.LimpezaDados
{
    [Feature("Replace", "Limpeza de Dados")]
    [FeatureName("Replace")]
    public class Replace : IFeature
    {
        public string NomeExibicao => "Replace";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col { get; set; }
        public string old { get; set; }
        public string value { get; set; }
    }
}
