using System;
using System.Collections.Generic;

namespace IntegradorDominio.DataFrameModel
{
    public class DataFrame
    {
        private readonly List<ColunaBase> _colunas = new();
        private readonly Dictionary<string, int> _colunaIndex = new(StringComparer.Ordinal);

        public string NomeContexto { get; set; } = string.Empty;

        public int QuantidadeLinhas => _colunas.Count == 0 ? 0 : _colunas[0].Quantidade;

        public List<ColunaBase> Colunas => _colunas;

        public Dictionary<string, int> ColunaIndex => _colunaIndex;

        // 🔥 evita realloc de List<T> quando possível
        public void AdicionarColuna<T>(string nome, List<T?> dados)
        {
            if (_colunas.Count > 0 && dados.Count != QuantidadeLinhas)
                throw new Exception("Column size mismatch");

            var col = new Coluna<T?>(nome, dados);

            _colunaIndex[nome] = _colunas.Count;
            _colunas.Add(col);
        }

        public void RenomearColunas(string antigoNome, string novoNome)
        {
            if (!_colunaIndex.TryGetValue(antigoNome, out int index))
                throw new Exception($"Coluna '{antigoNome}' não encontrada.");

            _colunas[index].Nome = novoNome;

            _colunaIndex.Remove(antigoNome);
            _colunaIndex[novoNome] = index;
        }

        public Coluna<T?>? PegarColuna<T>(string nome)
        {
            return _colunaIndex.TryGetValue(nome, out int index)
                ? _colunas[index] as Coluna<T?>
                : null;
        }

        public ColunaBase? PegarColunaBase(string nome)
        {
            return _colunaIndex.TryGetValue(nome, out int index)
                ? _colunas[index]
                : null;
        }

        // 🔥 NÃO recria coluna inteira (grande ganho)
        public void AlterarColuna<T>(string nome, List<T?> valor)
        {
            if (_colunaIndex.TryGetValue(nome, out int index))
            {
                var col = (Coluna<T?>)_colunas[index];

                // reutiliza memória se possível
                col.SubstituirDados(valor);
            }
            else
            {
                AdicionarColuna(nome, valor);
            }
        }

        public void AlterarColuna<T>(string nome, List<T?> valor, Type tipo)
        {
            if (_colunaIndex.TryGetValue(nome, out int index))
            {
                var nova = new Coluna<T?>(nome, valor);
                _colunas[index] = nova;
                return;
            }

            var tipoLista = typeof(List<>).MakeGenericType(tipo);
            var listaConvertida = (System.Collections.IList)Activator.CreateInstance(tipoLista)!;

            foreach (var v in valor)
            {
                listaConvertida.Add(v == null ? null : Convert.ChangeType(v, tipo));
            }

            var tipoColuna = typeof(Coluna<>).MakeGenericType(tipo);
            var novaColuna = (ColunaBase)Activator.CreateInstance(tipoColuna, nome, listaConvertida)!;

            _colunas.Add(novaColuna);
            _colunaIndex[nome] = _colunas.Count - 1;
        }
    }
}