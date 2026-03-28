using IntegradorDominio.DataFrameModel;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesAritmeticas
{
    public class Dividir : IFeature
    {
        public string NomeExibicao => "Dividir";
        public string NomeCodigo => "Div";

        public string NomeColunaSaida { get; set; }
        public Coluna<float> ColunaEsquerda;
        public Coluna<float> ColunaDireita;

        public Dividir() { }

        public Dividir(string nomeColunaSaida, Coluna<float> colunaEsquerda, Coluna<float> colunaDireita)
        {
            NomeColunaSaida = nomeColunaSaida;
            ColunaEsquerda = colunaEsquerda;
            ColunaDireita = colunaDireita;
        }
    }
}
