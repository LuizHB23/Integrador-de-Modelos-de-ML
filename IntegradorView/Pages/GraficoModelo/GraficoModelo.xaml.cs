using IntegradorViewModel.Pages.GraficoModelo;
using System.Windows.Controls;

namespace IntegradorView.Pages.GraficoModelo
{
    /// <summary>
    /// Interação lógica para GraficoClassificacao.xam
    /// </summary>
    public partial class GraficoModelo : Page
    {
        public GraficoModelo()
        {
            InitializeComponent();
            DataContext = App.GetService<GraficoModeloViewModel>();
        }
    }
}
