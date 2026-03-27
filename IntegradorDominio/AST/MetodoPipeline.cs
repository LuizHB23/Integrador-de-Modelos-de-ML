using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.AST
{
    public class MetodoPipeline
    {
        public string Nome { get; set; }
        public List<ComandoMetodoPipeline> Comandos { get; set; }

        public MetodoPipeline(string nome)
        {
            Nome = nome;
            Comandos = new();
        }
    }
}
