using IntegradorDominio.Attributes;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesAritmeticas
{
    [FeatureName("Dividir")]
    [FeatureName("Div")]
    public class Dividir : IFeature
    {
        public string NomeExibicao => "Dividir";

        public string exit { get; set; }
        public string left { get; set; }
        public string right { get; set; }
        public Dictionary<string, object?>? Contexto { get; set; }

        public Dividir() { }

        public Dividir(string nomeColunaSaida, string colunaEsquerda, string colunaDireita)
        {
            exit = nomeColunaSaida;
            left = colunaEsquerda;
            right = colunaDireita;
        }
    }
}
