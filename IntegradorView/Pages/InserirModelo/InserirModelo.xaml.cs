using InetradorAplicacao.Gerenciador;
using IntegradorView.Pages.PrincipalModelo;
using IntegradorViewModel.Pages.InserirModelo;
using System.Windows;
using System.Windows.Controls;

namespace IntegradorView.Pages.InserirModelo
{
    public partial class InserirModelo : UserControl
    {

        public InserirModelo()
        {
            InitializeComponent();
            DataContext = App.GetService<InserirModeloViewModel>();
        }
    }
}
