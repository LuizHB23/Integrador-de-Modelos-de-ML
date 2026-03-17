namespace IntegradorAplicacao.ConversorJson
{
    public interface IConverteJson<T> where T : class
    {
        void EscreverJson(T objeto);
        void ConverteJson(T objeto);
        T CarregarJson(string caminho);
    }
}
