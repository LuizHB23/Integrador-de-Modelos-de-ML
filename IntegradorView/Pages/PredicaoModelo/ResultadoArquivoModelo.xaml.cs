using IntegradorViewModel.Pages.PredicaoModelo;
using System.Windows.Controls;

namespace IntegradorView.Pages.PredicaoModelo
{
    /// <summary>
    /// Interação lógica para ResultadoArquivoModelo.xam
    /// </summary>
    public partial class ResultadoArquivoModelo : UserControl
    {
        public ResultadoArquivoModelo()
        {
            InitializeComponent();
            DataContext = App.GetService<ResultadoArquivoModeloViewModel>();
        }
    }
}
