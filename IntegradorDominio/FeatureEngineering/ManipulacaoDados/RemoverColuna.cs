using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.ManipulacaoDados
{
    [FeatureName("Drop")]
    public class RemoverColuna : IFeature
    {
        public string NomeExibicao => "Remover Coluna";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col {  get; set; }
    }
}
