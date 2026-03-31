using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.LimpezaDados
{
    public class ResetarIndex : IFeature
    {
        public string NomeExibicao => "Resetar Index";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
