using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesExponenciais
{
    [Feature("Log10", "Operações Exponenciais")]
    [FeatureName("Log10")]
    public class Log10 : IFeature
    {
        public string NomeExibicao => "Log10";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col {  get; set; }
    }
}
