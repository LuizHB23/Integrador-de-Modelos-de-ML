using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesExponenciais
{
    public class RaizQuadrada : IFeature
    {
        public string NomeExibicao => "Raiz Quadrada";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
