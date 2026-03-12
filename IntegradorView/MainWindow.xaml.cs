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
        }
    }
}