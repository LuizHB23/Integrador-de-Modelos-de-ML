using IntegradorViewModel.Pages.PredicaoModelo;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace IntegradorView.Pages.PredicaoModelo
{
    /// <summary>
    /// Interação lógica para PredicaoModelo.xam
    /// </summary>
    public partial class PreparacaoModelo : UserControl
    {
        public PreparacaoModelo()
        {
            InitializeComponent();
            DataContext = App.GetService<PreparacaoModeloViewModel>();
        }
    }
}
