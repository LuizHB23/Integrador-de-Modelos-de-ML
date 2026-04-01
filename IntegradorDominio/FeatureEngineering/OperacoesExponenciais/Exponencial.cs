using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesExponenciais
{
    [FeatureName("Exp")]
    public class Exponencial : IFeature
    {
        public string NomeExibicao => "Exponencial";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col { get; set; }
    }
}
