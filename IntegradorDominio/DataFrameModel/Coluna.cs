
namespace IntegradorDominio.DataFrameModel
{
    public class Coluna<T> : ColunaBase
    {
        private T?[] _dados;
        private int _count;

        public override int Quantidade => _count;
        public T?[] Dados => _dados;

        public Coluna(string nome, List<T?> dados) : base(nome, typeof(T))
        {
            _dados = dados.ToArray();
            _count = _dados.Length;
        }

        public Coluna(string nome, T?[] dados) : base(nome, typeof(T))
        {
            _dados = dados;
            _count = dados.Length;
        }

        public override object? PegarValor(int index)
        {
            return _dados[index];
        }

        public override void AdicionaValor(object? valor)
        {
            EnsureCapacity();
            _dados[_count++] = (T?)valor;
        }

        public override void InjetarValor(int index, object? valor)
        {
            _dados[index] = (T?)valor;
        }

        public override void SubstituirDados(object novos)
        {
            if (novos is List<T?> lista)
            {
                if (lista.Count <= _dados.Length)
                {
                    for (int i = 0; i < lista.Count; i++)
                        _dados[i] = lista[i];

                    _count = lista.Count;
                    return;
                }

                _dados = lista.ToArray();
                _count = _dados.Length;
            }
            else if (novos is T?[] array)
            {
                _dados = array;
                _count = array.Length;
            }
            else
            {
                throw new InvalidCastException($"Tipo inválido para SubstituirDados: {novos.GetType()}");
            }
        }

        public override ColunaBase Clonar()
        {
            var copy = new T?[_count];
            Array.Copy(_dados, copy, _count);
            return new Coluna<T>(Nome, copy);
        }

        public T? Get(int index) => _dados[index];

        public Span<T?> PegarColunaSpan()
        {
            return _dados.AsSpan(0, _count);
        }

        public T?[] PegarColuna()
        {
            var copy = new T?[_count];
            Array.Copy(_dados, copy, _count);
            return copy;
        }
        public override void Reordenar(int[] indices)
        {
            var novo = new T[_dados.Length];

            for (int i = 0; i < indices.Length; i++)
                novo[i] = _dados[indices[i]];

            _dados = novo;
        }

        private void EnsureCapacity()
        {
            if (_count < _dados.Length)
                return;

            var newSize = _dados.Length == 0 ? 4 : _dados.Length * 2;
            Array.Resize(ref _dados, newSize);
        }
    }
}