using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.AgrupamentoDados
{
    public class Join : IFeature
    {
        public string NomeExibicao => "Join";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
