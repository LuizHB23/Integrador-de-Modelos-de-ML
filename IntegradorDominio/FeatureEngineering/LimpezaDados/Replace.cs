using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.LimpezaDados
{
    public class Replace : IFeature
    {
        public string NomeExibicao => "Replace";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
