using IntegradorViewModel.Pages.ConfiguracaoModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using System.Windows;
using System.Windows.Controls;

namespace IntegradorView.Pages.PrincipalModelo
{
    /// <summary>
    /// Interação lógica para Home.xam
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();

            Loaded += async (_, __) =>
            {
                if (DataContext is HomeViewModel vm)
                {
                    await vm.InicializarAsync();
                }
            };
        }
    }
}
