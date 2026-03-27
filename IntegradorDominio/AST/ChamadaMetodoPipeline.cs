using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IntegradorDominio.AST
{
    public class ChamadaMetodoPipeline : ExpressaoMetodoPipeline
    {
        public string ObjetoInicial { get; set; }
        public List<MetodoChainPipeline> Metodos { get; set; }

        public ChamadaMetodoPipeline(string objetoInicial)
        {
            ObjetoInicial = objetoInicial;
            Metodos = new();
        }
    }
}
