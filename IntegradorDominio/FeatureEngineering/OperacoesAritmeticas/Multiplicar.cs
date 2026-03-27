using IntegradorDominio.DataFrameModel;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesAritmeticas
{
    public class Multiplicar : IFeature
    {
        public string NomeExibicao => "Multiplicar";
        public string NomeCodigo => "Mult";

        public string NomeColunaSaida { get; set; }
        public Coluna<float> ColunaEsquerda;
        public Coluna<float> ColunaDireita;

        public Multiplicar(string nomeColunaSaida, Coluna<float> colunaEsquerda, Coluna<float> colunaDireita)
        {
            NomeColunaSaida = nomeColunaSaida;
            ColunaEsquerda = colunaEsquerda;
            ColunaDireita = colunaDireita;
        }
    }
}
