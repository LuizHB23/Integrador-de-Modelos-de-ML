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
        public string NomeCodigo => "Mult";

        public DataFrame df { get; set; }
        public string exit { get; set; }
        //public Coluna<float> left 
        //{ get; 
            
        //  set
        //  {
        //        if(float.GetType(value))
        //  }
        //}
        public Coluna<float> right { get; set; }

        public Multiplicar() { }
        public Multiplicar(DataFrame df, string nomeColunaSaida, Coluna<float> colunaEsquerda, Coluna<float> colunaDireita)
        {
            exit = nomeColunaSaida;
            //left = colunaEsquerda;
            right = colunaDireita;
            this.df = df;
        }
    }
}
