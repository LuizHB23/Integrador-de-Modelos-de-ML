using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.AST
{
    public class MetodoChainPipeline
    {
        public string? Nome { get; set; }
        public List<ArgumentoMetodoPipeline> Argumentos { get; set; } = new();
    }
}
