using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorViewModel.ItensViewModel
{
    public class FuncaoItemViewModel
    {
        public int Posicao { get; set; }
        public string NomeFuncao { get; private set; } = string.Empty;
        public List<string> Codigo { get; set; } = new();

        public FuncaoItemViewModel(int posicao, string nomeFuncao, List<string> codigo)
        {
            Posicao = posicao;
            NomeFuncao = nomeFuncao;
            Codigo = codigo;
        }
    }
}
