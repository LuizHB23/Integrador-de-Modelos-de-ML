using System;
using System.Collections.Generic;
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

namespace Integrador_de_Modelos_de_ML.Pages.PredicaoModelo
{
    /// <summary>
    /// Interação lógica para PredicaoModelo.xam
    /// </summary>
    public partial class PreparacaoModelo : Page
    {
        public PreparacaoModelo()
        {
            InitializeComponent();
        }

        private void BtnProcessamento_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new ResultadoArquivoModelo());
    }
}
