namespace IntegradorDominio.DataFrameModel
{
    public abstract class ColunaBase
    {
        public string Nome { get; set; }
        public Type TipoDado { get; }
        public abstract int Quantidade { get; }

        protected ColunaBase(string nome, Type type)
        {
            Nome = nome;
            TipoDado = type;
        }

        public abstract ColunaBase Clonar();
        public abstract object PegarValor(int index);
        public abstract void AdicionaValor(object? valor);
        public abstract void SubstituirDados(object novos);
        public abstract void InjetarValor(int index, object? valor);

    }
}
