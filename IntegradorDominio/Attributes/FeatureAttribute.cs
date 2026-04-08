using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureAttribute : Attribute
    {
        public string Nome { get; }
        public string Categoria { get; }

        public FeatureAttribute(string nome, string categoria)
        {
            Nome = nome;
            Categoria = categoria;
        }
    }
}
