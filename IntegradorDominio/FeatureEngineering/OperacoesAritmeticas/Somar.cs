using IntegradorDominio.DataFrameModel;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesAritmeticas
{
    public class Somar : IFeature
    {
        public string NomeExibicao => "Somar";
        public string NomeCodigo => "Sum";

        public string NomeColunaSaida { get; set; }
        public Coluna<float> ColunaEsquerda;
        public Coluna<float> ColunaDireita;

        public Somar() { }

        public Somar(string nomeColunaSaida, Coluna<float> colunaEsquerda, Coluna<float> colunaDireita)
        {
            NomeColunaSaida = nomeColunaSaida;
            ColunaEsquerda = colunaEsquerda;
            ColunaDireita = colunaDireita;
        }
    }
}
