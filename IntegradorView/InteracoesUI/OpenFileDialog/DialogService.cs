using IntegradorViewModel.Interfaces;

namespace IntegradorView.InteracoesUI.OpenFileDialog
{
    public class DialogService : IDialogService
    {
        public string? GetCaminhoArquivo()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Selecione um arquivo",
                Multiselect = false,
                Filter = "Todos os arquivos (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return null;
        }
    }
}
