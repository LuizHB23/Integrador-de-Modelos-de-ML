using IntegradorView.Pages.GraficoModelo;
using IntegradorView.Pages.InserirModelo;
using IntegradorView.Pages.PrincipalModelo;
using IntegradorViewModel.JanelaModelo;
using System.Windows;
using System.Windows.Controls;

namespace IntegradorView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.GetService<MainWindowViewModel>();
            //MainFrame.Navigate(new Home());
        }

        private void BtnInserirModelo_Click(object sender, RoutedEventArgs e) => TrocarPaginas(new InserirModelo());

        private void BtnHome_Click(object sender, RoutedEventArgs e) => TrocarPaginas(new Home());
        private void BtnGraficoModelo_Click(object sender, RoutedEventArgs e) => TrocarPaginas(new GraficoModelo());

        private void TrocarPaginas(Page pagina)
        {
            //MainFrame.Navigate(pagina);
        }

    }
}