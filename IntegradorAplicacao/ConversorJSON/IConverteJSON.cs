namespace IntegradorAplicacao.ConversorJSON
{
    public interface IConverteJSON<T> where T : class
    {
        void ConverteJSON(T objeto);
    }
}
