using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.LimpezaDados
{
    [Feature("DropNa", "Limpeza de Dados")]
    [FeatureName("DropNa")]
    public class RemoverNulo : IFeature
    {
        public string NomeExibicao => "Remover Nulo";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
