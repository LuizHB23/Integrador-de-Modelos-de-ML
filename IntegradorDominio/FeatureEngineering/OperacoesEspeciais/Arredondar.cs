using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEspeciais
{
    public class Arredondar : IFeature
    {
        public string NomeExibicao => "Arredondar";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
