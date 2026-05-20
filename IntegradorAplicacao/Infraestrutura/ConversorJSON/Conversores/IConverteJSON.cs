namespace IntegradorAplicacao.Infraestrutura.ConversorJson.Conversores
{
    public interface IConverteJson<T> where T : class
    {
        void ConverteJson(T objeto);
        T CarregarJson(string caminho);
    }
}
