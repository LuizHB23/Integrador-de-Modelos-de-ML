using InetradorAplicacao.Gerenciador;
using IntegradorView.Pages.PrincipalModelo;
using IntegradorViewModel.Pages.InserirModelo;
using System.Windows;
using System.Windows.Controls;

namespace IntegradorView.Pages.InserirModelo
{
    public partial class InserirModelo : UserControl
    {
        private IGerenciador _gereciador;
        // private string? caminhoModelo;

        public InserirModelo()
        {
            InitializeComponent();
            DataContext = App.GetService<InserirModeloViewModel>();
            _gereciador = new ModeloGerenciador();
        }
    }
}
