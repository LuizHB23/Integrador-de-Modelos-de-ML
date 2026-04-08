using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.LimpezaDados
{
    [Feature("Filter", "Limpeza de Dados")]
    [FeatureName("Filter")]
    public class Filtrar : IFeature
    {
        public string NomeExibicao => "Filtrar";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string condition { get; set; }
    }
}
