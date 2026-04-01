using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace IntegradorDominio.FeatureEngineering.MapearLinhas.CasosLinha
{
    public class ForMap : NodeMap
    {
        public string Condicao { get; set; }
        public List<NodeMap> Corpo { get; set; }

        public ForMap(string condicao, List<NodeMap> corpo)
        {
            Condicao = condicao;
            Corpo = corpo;
        }
    }
}
