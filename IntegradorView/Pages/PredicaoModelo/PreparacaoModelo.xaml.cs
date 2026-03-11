using IntegradorViewModel.PredicaoModelo;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace IntegradorView.Pages.PredicaoModelo
{
    /// <summary>
    /// Interação lógica para PredicaoModelo.xam
    /// </summary>
    public partial class PreparacaoModelo : Page
    {
        public PreparacaoModelo()
        {
            InitializeComponent();
            DataContext = new PreparacaoModeloViewModel();
        }

        private void BtnProcessamento_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new ResultadoArquivoModelo());
    }
}
