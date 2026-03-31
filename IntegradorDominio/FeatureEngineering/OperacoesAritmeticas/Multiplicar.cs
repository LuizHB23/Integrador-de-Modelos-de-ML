using IntegradorDominio.Attributes;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesAritmeticas
{
    [FeatureName("Multiplicar")]
    [FeatureName("Mult")]
    public class Multiplicar : IFeature
    {
        public string NomeExibicao => "Multiplicar";
        public Dictionary<string, object?>? Contexto { get; set; }

        public string exit { get; set; }
        public string left { get; set; }
        public string right { get; set; }

        public Multiplicar() { }
        public Multiplicar(string nomeColunaSaida, string colunaEsquerda, string colunaDireita)
        {
            exit = nomeColunaSaida;
            left = colunaEsquerda;
            right = colunaDireita;
        }
    }
}
