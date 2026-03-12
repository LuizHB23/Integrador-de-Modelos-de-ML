using InetradorAplicacao.Gerenciador;
using IntegradorView.Pages.PrincipalModelo;
using IntegradorViewModel.Pages.InserirModelo;
using System.Windows;
using System.Windows.Controls;

namespace IntegradorView.Pages.InserirModelo
{
    public partial class InserirModelo : Page
    {
        private IGerenciador _gereciador;
        // private string? caminhoModelo;

        public InserirModelo()
        {
            InitializeComponent();
            DataContext = new InserirModeloViewModel();
            _gereciador = new ModeloGerenciador();
        }

        private void BtnCriarModelo_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new ConfigurarSchema());
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new Home());
    }
}
