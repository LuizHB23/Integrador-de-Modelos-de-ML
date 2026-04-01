using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEstatisticas
{
    [FeatureName("Std")]
    public class DesvioPadrao : IFeature
    {
        public string NomeExibicao => "Desvio Padrão";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col { get; set; }
    }
}
