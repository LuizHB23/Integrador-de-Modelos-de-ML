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

        public string exit { get; set; }
        public string left { get; set; }
        public string right { get; set; }
        public Dictionary<string, object?>? Contexto { get; set; }

        public Somar() { }

        public Somar(string nomeColunaSaida, string colunaEsquerda, string colunaDireita)
        {
            exit = nomeColunaSaida;
            left = colunaEsquerda;
            right = colunaDireita;
        }
    }
}
