namespace IntegradorViewModel.Shared.Context
{
    public interface IContext<T> where T : class
    {
        void EnviaMensagem(T mensagem);
        T RecebeMensagem();
    }
}
