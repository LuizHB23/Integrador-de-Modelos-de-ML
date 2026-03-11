using IntegradorView.Pages.PredicaoModelo;
using IntegradorViewModel.PrincipalModelo;
using System.Windows;
using System.Windows.Controls;

namespace IntegradorView.Pages.PrincipalModelo
{
    /// <summary>
    /// Interação lógica para Home.xam
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
            DataContext = App.GetService<HomeViewModel>();
        }

        private void BtnModelo_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new PreparacaoModelo());
    }
}
