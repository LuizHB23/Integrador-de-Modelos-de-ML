namespace IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson.ConverteJson
{
    public interface IConverteJson<T> where T : class
    {
        void ConverteJson(T objeto);
        T CarregarJson(string caminho);
    }
}
