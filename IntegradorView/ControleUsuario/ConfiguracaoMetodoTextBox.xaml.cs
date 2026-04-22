using IntegradorViewModel.ControleUsuario;
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
