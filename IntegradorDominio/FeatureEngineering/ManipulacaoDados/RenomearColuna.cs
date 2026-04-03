using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.ManipulacaoDados
{
    [FeatureName("Rename")]
    public class RenomearColuna : IFeature
    {
        public string NomeExibicao => "Renomear Coluna";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col {  get; set; }
        public string name { get; set; }
    }
}
