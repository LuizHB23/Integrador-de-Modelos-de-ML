using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesExponenciais
{
    [Feature("Pow", "Operações Exponenciais")]
    [FeatureName("Pow")]
    public class Potencia : IFeature
    {
        public string NomeExibicao => "Potência";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col { get; set; }
        public string value { get; set; }
    }
}
