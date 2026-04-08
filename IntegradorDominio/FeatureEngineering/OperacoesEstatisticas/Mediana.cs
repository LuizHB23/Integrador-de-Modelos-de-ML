using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEstatisticas
{
    [Feature("Median", "Operações Estatísticas")]
    [FeatureName("Median")]
    public class Mediana : IFeature
    {
        public string NomeExibicao => "Mediana";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col { get; set; }
    }
}
