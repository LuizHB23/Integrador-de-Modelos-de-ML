using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEstatisticas
{
    [Feature("Mode", "Operações Estatísticas")]
    [FeatureName("Mode")]
    public class Moda : IFeature
    {
        public string NomeExibicao => "Moda";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col { get; set; }
    }
}
