using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.LimpezaDados
{
    public class Filtrar : IFeature
    {
        public string NomeExibicao => "Filtrar";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
