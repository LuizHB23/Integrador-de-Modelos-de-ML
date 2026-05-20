using IntegradorViewModel.ControleUsuario;
using System.Windows.Controls;

namespace IntegradorView.ControleUsuario
{
    /// <summary>
    /// Interação lógica para ConfiguracaoMetodoTextBox.xam
    /// </summary>
    public partial class ConfiguracaoMetodoTextBox : UserControl
    {
        public ConfiguracaoMetodoTextBox()
        {
            InitializeComponent();

            Loaded += async (_, __) =>
            {
                if (DataContext is ConfiguracaoPipelineTextBoxViewModel mtb)
                {
                    await Task.Run(() => mtb.GuardaEstado());
                    mtb.AtualizaTabela(mtb.CarregarDados());
                }
            };
        }
    }
}
