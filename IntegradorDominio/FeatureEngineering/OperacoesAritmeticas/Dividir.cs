using IntegradorDominio.Attributes;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesAritmeticas
{
    [Feature("Div", "Operações Aritméticas")]
    [FeatureName("Div")]
    public class Dividir : IFeature
    {
        public string NomeExibicao => "Dividir";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string left { get; set; }
        public string right { get; set; }
        public string value { get; set; }
        public string exit { get; set; }
    }
}
