namespace IntegradorViewModel.Shared.Interfaces
{
    public interface INotificationService
    {
        void Notify(string mensagem);
        void Notify(string mensagem, string acao, Action callback);
    }
}
