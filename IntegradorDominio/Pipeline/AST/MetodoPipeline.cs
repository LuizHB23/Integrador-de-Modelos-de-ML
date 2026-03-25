using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.AST
{
    public class MetodoPipeline
    {
        public string? Nome { get; set; }
        public List<ComandoMetodoPipeline> Comandos { get; set; } = new();
    }
}
