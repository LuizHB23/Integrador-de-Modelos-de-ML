using IntegradorViewModel.PredicaoModelo;
using System.Windows.Controls;

namespace IntegradorView.Pages.PredicaoModelo
{
    /// <summary>
    /// Interação lógica para Graficos.xam
    /// </summary>
    public partial class AjusteModelo : Page
    {
        public AjusteModelo()
        {
            InitializeComponent();
            DataContext = new AjusteModeloViewModel();
        }
    }
}
