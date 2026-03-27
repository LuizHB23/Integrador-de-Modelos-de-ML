using IntegradorDominio.DataFrameModel;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.OperacoesAritmeticas
{
    public class Mod : IFeature
    {
        public string NomeExibicao => "Mod";
        public string NomeCodigo => "Mod";

        public string NomeColunaSaida { get; set; }
        public Coluna<float> Coluna;
        public int Divisor;

        public Mod(string nomeColunaSaida, Coluna<float> coluna, int divisor)
        {
            Coluna = coluna;
            Divisor = divisor;

            if(string.IsNullOrWhiteSpace(nomeColunaSaida))
            {
                NomeColunaSaida = coluna.Nome;
            }
            else
            {
                NomeColunaSaida = nomeColunaSaida;
            }
        }
    }
}
