using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IntegradorDominio.AST
{
    public class AtribuicaoMetodoPipeline : ComandoMetodoPipeline
    {
        public string Variavel { get; set; }
        public ChamadaMetodoPipeline ChamadaMetodo { get; set; }

        public AtribuicaoMetodoPipeline(string variavel, ChamadaMetodoPipeline chamadaMetodo)
        {
            Variavel = variavel;
            ChamadaMetodo = chamadaMetodo;
        }
    }
}
