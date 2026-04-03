using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.ManipulacaoDados
{
    [FeatureName("Copy")]
    public class Copiar : IFeature
    {
        public string NomeExibicao => "Copiar";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
