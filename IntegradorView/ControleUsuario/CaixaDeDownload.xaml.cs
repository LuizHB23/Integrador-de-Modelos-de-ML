using IntegradorView.InteracoesUI.Notification;
using IntegradorViewModel.Pages.PredicaoModelo;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IntegradorView.ControleUsuario
{
    /// <summary>
    /// Interação lógica para CaixaDeDownload.xam
    /// </summary>
    public partial class CaixaDeDownload : UserControl
    {
        public CaixaDeDownload()
        {
            InitializeComponent();

            var queue = App.GetService<SnackbarMessageQueue>();
            MeuSnackbar.MessageQueue = queue;
        }
    }
}
