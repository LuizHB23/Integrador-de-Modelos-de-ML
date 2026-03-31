using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesEstatisticas
{
    public class Mediana : IFeature
    {
        public string NomeExibicao => "Mediana";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
