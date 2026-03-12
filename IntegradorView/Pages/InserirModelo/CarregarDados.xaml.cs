using IntegradorView.Pages.PrincipalModelo;
using IntegradorViewModel.Pages.InserirModelo;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace IntegradorView.Pages.InserirModelo
{
    /// <summary>
    /// Interação lógica para CarregarDados.xam
    /// </summary>
    public partial class CarregarDados : Page
    {
        public CarregarDados()
        {
            InitializeComponent();
            DataContext = new CarregarDadosViewModel();
        }

        private void BtnContinuar_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new PipelineModelo());
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new Home());
    }
}
