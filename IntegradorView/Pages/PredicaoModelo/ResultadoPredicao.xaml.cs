using IntegradorViewModel.Pages.PredicaoModelo;
using System.Windows.Controls;

namespace IntegradorView.Pages.PredicaoModelo
{
    /// <summary>
    /// Interação lógica para ResultadoArquivoModelo.xam
    /// </summary>
    public partial class ResultadoPredicao : UserControl
    {
        public ResultadoPredicao()
        {
            InitializeComponent();
            DataContext = App.GetService<ResultadoPredicaoViewModel>();
        }
    }
}
