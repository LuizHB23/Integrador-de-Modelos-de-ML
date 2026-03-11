using IntegradorViewModel.PredicaoModelo;
using System.Windows.Controls;

namespace IntegradorView.Pages.PredicaoModelo
{
    /// <summary>
    /// Interação lógica para ResultadoArquivoModelo.xam
    /// </summary>
    public partial class ResultadoArquivoModelo : Page
    {
        public ResultadoArquivoModelo()
        {
            InitializeComponent();
            DataContext = new ResultadoArquivoModeloViewModel();
        }
    }
}
