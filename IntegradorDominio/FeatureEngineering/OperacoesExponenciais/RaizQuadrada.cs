using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesExponenciais
{
    [Feature("Sqrt", "Operações Exponenciais")]
    [FeatureName("Sqrt")]
    public class RaizQuadrada : IFeature
    {
        public string NomeExibicao => "Raiz Quadrada";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col { get; set; }
    }
}
