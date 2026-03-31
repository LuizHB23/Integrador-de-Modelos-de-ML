using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEstatisticas
{
    public class Percentil : IFeature
    {
        public string NomeExibicao => "Percentil";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
