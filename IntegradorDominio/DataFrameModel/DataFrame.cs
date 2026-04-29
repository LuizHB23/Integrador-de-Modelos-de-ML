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

        public void AlterarColuna<T>(string nome, List<T?> valor)
        {
            if (_colunaIndex.TryGetValue(nome, out int index))
            {
                var colunaAtual = _colunas[index];

                if (colunaAtual is Coluna<T?> col)
                {
                    col.SubstituirDados(valor);
                }
                else
                {
                    _colunas[index] = new Coluna<T?>(nome, valor);
                }
            }
            else
            {
                AdicionarColuna(nome, valor);
            }
        }

        public void AlterarColuna<T>(string nome, T?[] valor)
        {
            if (_colunaIndex.TryGetValue(nome, out int index))
            {
                var colunaAtual = _colunas[index];

                if (colunaAtual is Coluna<T?> col)
                {
                    col.SubstituirDados(valor); // 🔥 zero cópia
                }
                else
                {
                    _colunas[index] = new Coluna<T?>(nome, valor);
                }
            }
            else
            {
                var col = new Coluna<T?>(nome, valor);
                _colunaIndex[nome] = _colunas.Count;
                _colunas.Add(col);
            }
        }

        public void AlterarColuna<T>(string nome, List<T?> valor, Type tipo)
        {
            var novaColuna = CriarColunaComTipo(nome, valor, tipo);

            if (_colunaIndex.TryGetValue(nome, out int index))
            {
                _colunas[index] = novaColuna;
            }
            else
            {
                _colunas.Add(novaColuna);
                _colunaIndex[nome] = _colunas.Count - 1;
            }
        }

        private ColunaBase CriarColunaComTipo<T>(string nome, List<T?> valor, Type tipo)
        {
            var tipoLista = typeof(List<>).MakeGenericType(tipo);
            var listaConvertida = (System.Collections.IList)Activator.CreateInstance(tipoLista)!;

            foreach (var v in valor)
            {
                if (v == null)
                {
                    listaConvertida.Add(null);
                }
                else
                {
                    var tipoBase = Nullable.GetUnderlyingType(tipo) ?? tipo;
                    listaConvertida.Add(Convert.ChangeType(v, tipoBase));
                }
            }

            var tipoColuna = typeof(Coluna<>).MakeGenericType(tipo);
            return (ColunaBase)Activator.CreateInstance(tipoColuna, nome, listaConvertida)!;
        }
    }
}