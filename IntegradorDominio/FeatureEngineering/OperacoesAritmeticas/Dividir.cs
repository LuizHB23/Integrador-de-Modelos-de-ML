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

        public string Exit { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }

        public Dividir() { }

        public Dividir(string nomeColunaSaida, string colunaEsquerda, string colunaDireita)
        {
            Exit = nomeColunaSaida;
            Left = colunaEsquerda;
            Right = colunaDireita;
        }
    }
}
