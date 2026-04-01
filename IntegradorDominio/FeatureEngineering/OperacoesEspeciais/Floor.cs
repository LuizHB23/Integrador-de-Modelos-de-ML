using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEspeciais
{
    [FeatureName("Floor")]
    public class Floor : IFeature
    {
        public string NomeExibicao => "Floor";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col { get; set; }
    }
}
