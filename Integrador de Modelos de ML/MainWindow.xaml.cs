using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Integrador_de_Modelos_de_ML.Pages;
using Integrador_de_Modelos_de_ML.Pages.InserirModelo;

namespace Integrador_de_Modelos_de_ML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Home());
        }

        private void BtnInserirModelo_Click(object sender, RoutedEventArgs e)
        {
            TrocarPaginas(new InserirModelo());
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            TrocarPaginas(new Home());
        }

        private void TrocarPaginas(Page pagina)
        {
            MainFrame.Navigate(pagina);
        }
    }
}