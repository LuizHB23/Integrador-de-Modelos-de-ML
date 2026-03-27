using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.DataFrameModel
{
    public class DataFrame
    {
        private readonly List<ColunaBase> _colunas = new();
        private readonly Dictionary<string, int> _colunaIndex = new();

        public int QuantidadeLinhas { get; private set; }

        public void AddColumn<T>(string nome, T[] dados)
        {
            if (_colunas.Count > 0 && dados.Length != QuantidadeLinhas)
                throw new Exception("Column size mismatch");

            var col = new Coluna<T>(nome, dados);
            _colunaIndex[nome] = _colunas.Count;
            _colunas.Add(col);

            QuantidadeLinhas = dados.Length;
        }

        public Coluna<T> PegarColuna<T>(string nome)
        {
            return (Coluna<T>)_colunas[_colunaIndex[nome]];
        }
    }
}
