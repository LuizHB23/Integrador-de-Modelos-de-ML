using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.LimpezaDados
{
    [Feature("DropDuplicates", "Limpeza de Dados")]
    [FeatureName("DropDuplicates")]
    public class RemoverDuplicados : IFeature
    {
        public string NomeExibicao => "Remover Duplicados";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
