using IntegradorViewModel.Shared.Interfaces;
using System.Windows;

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

        public void ShowMessage(string message, string title = "Aviso")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public bool Confirm(string message, string title = "Confirmação")
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
    }
}
