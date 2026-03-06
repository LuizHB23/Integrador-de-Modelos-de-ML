using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.WPF
{
    public class ColunaSchema
    {
        public string NomeColuna { get; set; }
        public string TipoDados { get; set; }
        public string Finalidade { get; set; }
        public bool IsCategorico { get; set; }
    }
}
