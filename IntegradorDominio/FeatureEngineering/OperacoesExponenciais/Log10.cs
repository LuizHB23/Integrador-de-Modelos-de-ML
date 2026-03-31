using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesExponenciais
{
    public class Log10 : IFeature
    {
        public string NomeExibicao => "Log10";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
