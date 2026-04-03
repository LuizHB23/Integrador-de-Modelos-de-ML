using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.ManipulacaoDados
{
    [FeatureName("Select")]
    public class SelecionarColuna : IFeature
    {
        public string NomeExibicao => "Selecionar Coluna";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col {  get; set; }
    }
}
