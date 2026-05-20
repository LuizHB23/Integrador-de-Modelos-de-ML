namespace IntegradorDominio.FeatureEngineering.MapearLinhas.Variavel
{
    public class Variavel<T>
    {
        private List<T?> _valores;

        public string Nome { get; set; }
        public int Quantidade { get; set; }

        public Variavel(string nome, int tamanho)
        {
            _valores = new List<T?>(new T?[tamanho]);
            Nome = nome;
            Quantidade = tamanho;
        }

        public object? PegarValor(int index)
            => _valores[index];

        public  void AdicionaValor(object? valor)
            => _valores.Add((T?)valor);

        public void InjetarValor(int index, object? valor)
            => _valores[index] = (T?)valor;

        public T? Get(int index)
            => _valores[index];
    }
}
