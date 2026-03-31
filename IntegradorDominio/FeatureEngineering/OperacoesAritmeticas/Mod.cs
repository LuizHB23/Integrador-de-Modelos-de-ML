using IntegradorDominio.Attributes;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesAritmeticas
{
    [FeatureName("Mod")]
    public class Mod : IFeature
    {
        public string NomeExibicao => "Mod";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string col { get; set; }
        public string value { get; set; }
        public string exit { get; set; }
    }
}
