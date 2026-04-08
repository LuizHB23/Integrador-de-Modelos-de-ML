using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEspeciais
{
    [Feature("Abs", "Operações Especiais")]
    [FeatureName("Abs")]
    public class Absoluto : IFeature
    {
        public string NomeExibicao => "Absoluto";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col { get; set; }

    }
}
