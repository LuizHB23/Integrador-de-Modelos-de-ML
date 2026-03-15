namespace IntegradorViewModel.Interfaces
{
    public interface IDialogService
    {
        string? GetCaminhoArquivo();
        void ShowMessage(string message, string title = "Aviso");
        bool Confirm(string message, string title = "Confirmação");
    }
}
