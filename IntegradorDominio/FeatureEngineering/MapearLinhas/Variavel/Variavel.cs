using IntegradorDominio.DataFrameModel;

namespace IntegradorDominio.FeatureEngineering.MapearLinhas.Variavel
{
    public class Variavel<T> : ColunaBase
    {
        private List<T?> _valores;

        public override int Quantidade => _valores.Count;

        public Variavel(string nome, int tamanho) : base(nome, typeof(T))
        {
            _valores = new List<T?>(new T?[tamanho]);
        }

        public override object? PegarValor(int index)
            => _valores[index];

        public override void AdicionaValor(object? valor)
            => _valores.Add((T?)valor);

        public override void InjetarValor(int index, object? valor)
            => _valores[index] = (T?)valor;

        public T? Get(int index)
            => _valores[index];
    }
}
