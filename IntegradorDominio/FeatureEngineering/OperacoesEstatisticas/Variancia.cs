using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEstatisticas
{
    [FeatureName("Var")]
    public class Variancia : IFeature
    {
        public string NomeExibicao => "Variância";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col { get; set; }
    }
}
