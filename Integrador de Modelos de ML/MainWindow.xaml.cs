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

namespace Integrador_de_Modelos_de_ML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Home? _home;
        private InserirModelo? _inserirModelo;
        public MainWindow()
        {
            InitializeComponent();
            IncializaPaginas();
            MainFrame.Navigate(_home);
        }

        private void BtnInserirModelo_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(_inserirModelo);
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(_home);
        }

        private void IncializaPaginas()
        {
            _home = new Home();
            _inserirModelo = new InserirModelo();
        }
    }
}