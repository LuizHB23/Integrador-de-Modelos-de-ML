using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.DataFrameModel
{
    public class Coluna<T> : ColunaBase
    {
        public List<T?> Dados;
        public override int Quantidade => Dados.Count;

        public Coluna(string nome, List<T?> dados) : base(nome, typeof(T))
        {
            Dados = dados;
        }
        public override object? PegarValor(int index)
        {
            return Dados[index]!;
        }

        public override void AdicionaValor(object? valor) => Dados.Add((T)valor!);

        public override void InjetarValor(int index, object? valor) => Dados[index] = (T)valor!;

        public T? Get(int index) => Dados[index];

    }
}
