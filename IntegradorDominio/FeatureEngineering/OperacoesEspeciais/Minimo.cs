using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEspeciais
{
    [Feature("Min", "Operações Especiais")]
    [FeatureName("Min")]
    public class Minimo : IFeature
    {
        public string NomeExibicao => "Mínimo";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col {  get; set; }
    }
}
