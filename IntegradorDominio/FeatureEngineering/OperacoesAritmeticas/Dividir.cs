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
        public string NomeCodigo => "Div";

        public string Exit { get; set; }
        public Coluna<float> Left;
        public Coluna<float> Right;

        public Dividir() { }

        public Dividir(string nomeColunaSaida, Coluna<float> colunaEsquerda, Coluna<float> colunaDireita)
        {
            Exit = nomeColunaSaida;
            Left = colunaEsquerda;
            Right = colunaDireita;
        }
    }
}
