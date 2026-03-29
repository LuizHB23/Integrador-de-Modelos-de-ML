using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.DataFrameModel
{
    public class Coluna<T> : ColunaBase
    {
        public T?[] Dados;
        public override int Quantidade => Dados.Length;

        public Coluna(string nome, T?[] dados) : base(nome, typeof(T))
        {
            Dados = dados;
        }
        public override object PegarValor(int index)
        {
            return Dados[index]!;
        }

        public T? Get(int index) => Dados[index];

    }
}
