using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.AST
{
    public class RetornoMetodoPipeline : ComandoMetodoPipeline
    {
        public string Variavel { get; set; }

        public RetornoMetodoPipeline(string variavel)
        {
            Variavel = variavel;
        }
    }
}
