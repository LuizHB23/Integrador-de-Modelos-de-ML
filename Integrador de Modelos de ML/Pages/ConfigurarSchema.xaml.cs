using IntegradorDominio.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Integrador_de_Modelos_de_ML.Pages
{
    /// <summary>
    /// Interação lógica para ConfigurarSchema.xam
    /// </summary>
    public partial class ConfigurarSchema : Page
    {
        public ConfigurarSchema()
        {
            InitializeComponent();
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
