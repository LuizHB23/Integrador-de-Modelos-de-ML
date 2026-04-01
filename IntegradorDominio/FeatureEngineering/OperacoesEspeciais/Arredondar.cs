using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEspeciais
{
    [FeatureName("Round")]
    public class Arredondar : IFeature
    {
        public string NomeExibicao => "Arredondar";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col { get; set; }
        public string value { get; set; }
    }
}
