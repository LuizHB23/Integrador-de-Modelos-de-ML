using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.LimpezaDados
{
    public class Clonar : IFeature
    {
        public string NomeExibicao => "Clonar";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
