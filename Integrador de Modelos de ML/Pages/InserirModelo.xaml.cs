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
    /// <summary>
    /// Interação lógica para InserirModelo.xam
    /// </summary>
    public partial class InserirModelo : Page
    {
        private IGerenciador _gereciador;
        private string? caminhoModelo;

        public InserirModelo()
        {
            InitializeComponent();
            _gereciador = new ModeloGerenciador();
        }

        public void BtnCarregarColunas_Click(string caminhoArquivo)
        {
            using(var sr =  new StreamReader(caminhoArquivo))
            {
                string texto = sr.ReadToEnd();
                var listaColunaSchema = JsonSerializer.Deserialize<List<ColunaSchema>>(texto);

                // O ItemsControl vai gerar um Card para cada item desta lista automaticamente!
                ListaColunasSchema.ItemsSource = listaColunaSchema;
            }
        }

        private void BtnProximo_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new PipelineModelo());
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new Home());

    }
}
