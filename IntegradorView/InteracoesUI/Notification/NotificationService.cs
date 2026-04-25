using IntegradorViewModel.Shared.Interfaces;
using MaterialDesignThemes.Wpf;

namespace IntegradorView.InteracoesUI.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly SnackbarMessageQueue _queue;

        public NotificationService(SnackbarMessageQueue queue)
        {
            _queue = queue;
        }

        public void Notify(string mensagem)
        {
            _queue.Enqueue(mensagem);
        }

        public void Notify(string mensagem, string acao, Action callback)
        {
            _queue.Enqueue(mensagem, acao, callback);
        }
    }
}
