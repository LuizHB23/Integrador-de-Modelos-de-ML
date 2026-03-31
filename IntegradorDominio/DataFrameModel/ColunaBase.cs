using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.DataFrameModel
{
    public abstract class ColunaBase
    {
        public string Nome { get; }
        public Type TipoDado { get; }
        public abstract int Quantidade { get; }

        protected ColunaBase(string nome, Type type)
        {
            Nome = nome;
            TipoDado = type;
        }

        public abstract object PegarValor(int index);
        public abstract void AdicionaValor(object? valor);
        public abstract void InjetarValor(int index, object? valor);

    }
}
