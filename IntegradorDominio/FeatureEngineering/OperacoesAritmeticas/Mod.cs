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
        public Dictionary<string, object?>? Contexto { get; set; }

        public string Exit { get; set; }
        public string Col { get; set; }
        public string Divisor { get; set; }

        public Mod() { }

        public Mod(string nomeColunaSaida, string coluna, string divisor)
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
