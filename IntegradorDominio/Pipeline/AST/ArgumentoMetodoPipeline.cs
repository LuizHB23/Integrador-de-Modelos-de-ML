using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.AST
{
    public class ArgumentoMetodoPipeline
    {
        public string? Nome { get; set; }
        public string Valor { get; set; }

        public ArgumentoMetodoPipeline(string? nome, string valor)
        {
            Nome = nome;
            Valor = valor;
        }
    }
}
