namespace IntegradorAplicacao.ConversorJson
{
    public interface IConverteJson<T> where T : class
    {
        void ConverteJson(T objeto);
    }
}
