using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.ManipulacaoDados
{
    [Feature("Create", "Manipulação de Dados")]
    [FeatureName("Create")]
    public class CriarColuna : IFeature
    {
        public string NomeExibicao => "Criar Coluna";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string name { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }
}
