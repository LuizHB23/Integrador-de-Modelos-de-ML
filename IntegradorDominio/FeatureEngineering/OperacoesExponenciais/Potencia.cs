using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesExponenciais
{
    public class Potencia : IFeature
    {
        public string NomeExibicao => "Potência";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
