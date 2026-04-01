using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesExponenciais
{
    [FeatureName("Log")]
    public class Logaritmo : IFeature
    {
        public string NomeExibicao => "Logaritmo";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col { get; set; }
    }
}
