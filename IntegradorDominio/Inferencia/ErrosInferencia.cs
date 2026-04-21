using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Inferencia
{
    public class ErrosInferencia
    {
        public string Id { get; set; }
        public string Erro { get; set; }
        public Dictionary<string, object> Outputs { get; set; }
    }
}
