using CommunityToolkit.Mvvm.ComponentModel;
using IntegradorAplicacao.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorViewModel.ItensViewModel
{
    public class SchemaItemViewModel
    {
        public int Posicao { get; set; }
        public string NomeColuna { get; private set; } = string.Empty;
        public string Finalidade { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public bool Categorico { get; set; }

        public SchemaItemViewModel(int posicao, string nomeColuna, string finalidade, string tipo, bool categorico)
        {
            Posicao = posicao;
            NomeColuna = nomeColuna;
            Finalidade = finalidade;
            Tipo = tipo;
            Categorico = categorico;
        }
    }
}
