using System;
using System.Collections.Generic;
using System.Text;

namespace InetradorAplicacao.Gerenciador
{
    public interface IGerenciador
    {
        public void Salvar(string caminho) { }
        public void Carregar(string caminho) { }
    }
}
