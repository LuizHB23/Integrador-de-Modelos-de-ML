using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IntegradorDominio.AST
{
    public class AtribuicaoMetodoPipeline : ComandoMetodoPipeline
    {
        public string Variavel { get; set; }
        public ExpressaoMetodoPipeline Expressao { get; set; }

        public AtribuicaoMetodoPipeline(string variavel, ExpressaoMetodoPipeline expressao)
        {
            Variavel = variavel;
            Expressao = expressao;
        }
    }
}
