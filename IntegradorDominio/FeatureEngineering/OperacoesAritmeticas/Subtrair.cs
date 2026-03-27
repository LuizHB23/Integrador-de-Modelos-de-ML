using IntegradorDominio.DataFrameModel;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesAritmeticas
{
    public class Subtrair : IFeature
    {
        public string NomeExibicao => "Subtrair";
        public string NomeCodigo => "Sub";

        public string NomeColunaSaida { get; set; }
        public Coluna<float> ColunaEsquerda;
        public Coluna<float> ColunaDireita;

        public Subtrair(string nomeColunaSaida, Coluna<float> colunaEsquerda, Coluna<float> colunaDireita)
        {
            NomeColunaSaida = nomeColunaSaida;
            ColunaEsquerda = colunaEsquerda;
            ColunaDireita = colunaDireita;
        }
    }
}
