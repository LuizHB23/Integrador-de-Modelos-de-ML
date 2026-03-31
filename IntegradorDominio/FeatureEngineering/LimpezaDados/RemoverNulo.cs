using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.LimpezaDados
{
    [FeatureName("DropNa")]
    public class RemoverNulo : IFeature
    {
        public string NomeExibicao => "Remover Nulo";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
