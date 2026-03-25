using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IntegradorDominio.Pipeline.AST
{
    public class AtribuicaoMetodoPipeline : ComandoMetodoPipeline
    {
        public string? Variavel { get; set; }
        public ExpressaoMetodoPipeline? Expressao { get; set; }
    }
}
