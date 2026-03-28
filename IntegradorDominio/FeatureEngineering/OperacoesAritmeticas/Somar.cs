using IntegradorDominio.Attributes;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesAritmeticas
{
    [FeatureName("Somar")]
    [FeatureName("Sum")]
    public class Somar : IFeature
    {
        public string NomeExibicao => "Somar";
        public string NomeCodigo => "Sum";

        public string exit { get; set; }
        public Coluna<float> left;
        public Coluna<float> right;

        public Somar() { }

        public Somar(string nomeColunaSaida, Coluna<float> colunaEsquerda, Coluna<float> colunaDireita)
        {
            exit = nomeColunaSaida;
            left = colunaEsquerda;
            right = colunaDireita;
        }
    }
}
