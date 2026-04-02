using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.MapearLinhas.CasosLinha
{
    public class LineMap : NodeMap
    {
        public string Linha { get; set; }

        public LineMap(string linha)
        {
            Linha = linha;
        }
    }
}
