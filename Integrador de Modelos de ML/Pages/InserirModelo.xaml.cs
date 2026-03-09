using InetradorAplicacao.DTO;
using InetradorAplicacao.Gerenciador;
using IntegradorDominio;
using IntegradorDominio.WPF;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;

namespace Integrador_de_Modelos_de_ML.Pages
{
    public partial class InserirModelo : Page
    {
        private IGerenciador _gereciador;
        // private string? caminhoModelo;

        public InserirModelo()
        {
            InitializeComponent();
            _gereciador = new ModeloGerenciador();
        }

        private void BtnCriarModelo_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new ConfigurarSchema());
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new Home());
    }
}
