
namespace IntegradorDominio.DataFrameModel
{
    public class Coluna<T> : ColunaBase
    {
        private T?[] _dados;
        private int _count;

        public override int Quantidade => _count;

        public Coluna(string nome, List<T?> dados) : base(nome, typeof(T))
        {
            _dados = dados.ToArray();
            _count = _dados.Length;
        }

        private Coluna(string nome, T?[] dados) : base(nome, typeof(T))
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

        public override void SubstituirDados(object novos)
        {
            var dadosNovos = (List<T?>)novos;

            if (dadosNovos.Count <= _dados.Length)
            {
                for (int i = 0; i < dadosNovos.Count; i++)
                    _dados[i] = dadosNovos[i];

                _count = dadosNovos.Count;
                return;
            }

            _dados = dadosNovos.ToArray();
            _count = _dados.Length;
        }

        public override void InjetarValor(int index, object? valor)
        {
            _dados[index] = (T?)valor;
        }

        public T? Get(int index) => _dados[index];

        public override ColunaBase Clonar()
        {
            var copy = new T?[_count];
            Array.Copy(_dados, copy, _count);
            return new Coluna<T>(Nome, copy);
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