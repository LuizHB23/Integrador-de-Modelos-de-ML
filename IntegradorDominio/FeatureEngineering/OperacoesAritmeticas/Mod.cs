using IntegradorDominio.Attributes;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesAritmeticas
{
    [FeatureName("Mod")]
    public class Mod : IFeature
    {
        public string NomeExibicao => "Mod";
        public string NomeCodigo => "Mod";

        public string Exit { get; set; }
        public Coluna<float> Col;
        public int Divisor;

        public Mod() { }

        public Mod(string nomeColunaSaida, Coluna<float> coluna, int divisor)
        {
            //Coluna = coluna;
            //Divisor = divisor;

            //if(string.IsNullOrWhiteSpace(nomeColunaSaida))
            //{
            //    Exit = coluna.Nome;
            //}
            //else
            //{
            //    NomeColunaSaida = nomeColunaSaida;
            //}
        }
    }
}
