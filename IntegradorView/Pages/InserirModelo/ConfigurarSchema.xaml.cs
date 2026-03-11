using IntegradorDominio.WPF;
using IntegradorView.Pages.PrincipalModelo;
using IntegradorViewModel.InserirModelo;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace IntegradorView.Pages.InserirModelo
{
    /// <summary>
    /// Interação lógica para ConfigurarSchema.xam
    /// </summary>
    public partial class ConfigurarSchema : Page
    {
        public ConfigurarSchema()
        {
            InitializeComponent();
            DataContext = new ConfigurarSchemaViewModel();
        }
        public void BtnCarregarColunas_Click(string caminhoArquivo)
        {
            using (var sr = new StreamReader(caminhoArquivo))
            {
                string texto = sr.ReadToEnd();
                var listaColunaSchema = JsonSerializer.Deserialize<List<ColunaSchema>>(texto);

                // O ItemsControl vai gerar um Card para cada item desta lista automaticamente!
                ListaColunasSchema.ItemsSource = listaColunaSchema;
            }
        }

        private void BtnProximo_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new CarregarDados());
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new Home());
    }
}
