using MaterialDesignThemes.Wpf;
using System.Windows.Controls;

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

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                var queue = App.GetService<SnackbarMessageQueue>();
                MeuSnackbar.MessageQueue = queue;
            }
        }
    }
}
