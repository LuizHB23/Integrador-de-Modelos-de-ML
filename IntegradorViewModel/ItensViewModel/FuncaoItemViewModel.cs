using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorViewModel.ItensViewModel
{
    public class FuncaoItemViewModel
    {
        public int Posicao { get; set; }
        public string NomeFuncao { get; private set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;

        public FuncaoItemViewModel(int posicao, string nomeFuncao, string codigo)
        {
            Posicao = posicao;
            NomeFuncao = nomeFuncao;
            Codigo = codigo;
        }
    }
}
