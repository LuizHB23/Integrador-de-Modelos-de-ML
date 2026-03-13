namespace InetradorAplicacao.Gerenciador
{
    public interface IGerenciador<T> where T : class
    {
        public string Salvar(T objeto);
        public void Carregar(T objeto);
    }
}
