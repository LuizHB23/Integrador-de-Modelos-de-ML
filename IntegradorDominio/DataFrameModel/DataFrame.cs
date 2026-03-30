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
        public List<ColunaBase> Colunas {  get => _colunas; }
        public Dictionary<string, int> ColunaIndex { get => _colunaIndex; }

        public void AdiconarColuna<T>(string nome, T?[] dados)
        {
            if (_colunas.Count > 0 && dados.Length != QuantidadeLinhas)
                throw new Exception("Column size mismatch");

            var col = new Coluna<T>(nome, dados);
            _colunaIndex[nome] = _colunas.Count;
            _colunas.Add(col);

            QuantidadeLinhas = dados.Length;
        }

        public Coluna<T?> PegarColuna<T>(string nome)
        {
            if (!_colunaIndex.TryGetValue(nome, out int index))
            {
                return null;
            }

            var coluna = _colunas[index];

            if (coluna is Coluna<T?> colunaTipada)
            {
                return colunaTipada;
            }

            return null;
        }

        public void AlteraColuna<T>(string nome, T?[] valor)
        {
            if (!_colunaIndex.TryGetValue(nome, out int index))
            {
                AdiconarColuna<T?>(nome, valor);
            }

            _colunas[index] = new Coluna<T?>(nome, valor);
        }
    }
}
