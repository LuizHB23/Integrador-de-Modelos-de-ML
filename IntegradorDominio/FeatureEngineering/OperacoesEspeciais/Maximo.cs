using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEspeciais
{
    [Feature("Max", "Operações Especiais")]
    [FeatureName("Max")]
    public class Maximo : IFeature
    {
        public string NomeExibicao => "Máximo";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col {  get; set; }
    }
}
