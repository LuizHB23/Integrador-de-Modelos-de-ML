using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.DataFrameModel
{
    public class DataFrame
    {
        private readonly List<ColunaBase> _colunas = new();
        private readonly Dictionary<string, int> _colunaIndex = new();

        public string NomeContexto { get; set; }
        public int QuantidadeLinhas { get => Colunas[0].Quantidade; }
        public List<ColunaBase> Colunas {  get => _colunas; }
        public Dictionary<string, int> ColunaIndex { get => _colunaIndex; }

        public void AdicionarColuna<T>(string nome, List<T?> dados)
        {
            if (_colunas.Count > 0 && dados.Count != QuantidadeLinhas)
                throw new Exception("Column size mismatch");

            var col = new Coluna<T?>(nome, dados);
            _colunaIndex[nome] = _colunas.Count;
            _colunas.Add(col);
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

        public ColunaBase? PegarColunaBase(string nome)
        {
            if (!_colunaIndex.TryGetValue(nome, out int index))
                return null;

            return _colunas[index];
        }

        public void AlterarColuna<T>(string nome, List<T?> valor)
        {
            if (!_colunaIndex.TryGetValue(nome, out int index))
            {
                AdicionarColuna<T?>(nome, valor);
            }
            else
            { 
                _colunas[index] = new Coluna<T?>(nome, valor);
            }
        }
    }
}
