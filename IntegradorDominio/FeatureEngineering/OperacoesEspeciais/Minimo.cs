using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEspeciais
{
    public class Minimo : IFeature
    {
        public string NomeExibicao => "Mínimo";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
