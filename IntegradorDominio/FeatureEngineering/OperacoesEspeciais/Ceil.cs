using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEspeciais
{
    public class Ceil : IFeature
    {
        public string NomeExibicao => "Ceil";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
