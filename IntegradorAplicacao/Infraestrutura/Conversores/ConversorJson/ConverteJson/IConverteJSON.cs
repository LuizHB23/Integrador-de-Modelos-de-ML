namespace IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson.ConverteJson
{
    public interface IConverteJson<T> where T : class
    {
        Task ConverteJsonAsync(T objeto);
        Task<T> CarregarJsonAsync(string caminho);
    }
}
