using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Models.Inferencia
{
    public class ErrosInferencia
    {
        public int IndexLinha { get; set; }
        public string Id { get; set; }
        public string Erro { get; set; }
        public Dictionary<string, object> Outputs { get; set; }
    }
}
