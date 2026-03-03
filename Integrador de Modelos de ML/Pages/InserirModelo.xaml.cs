using InetradorAplicacao.Gerenciador;
using IntegradorDominio;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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

        private void BtnCarregarModelo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Selecione um arquivo";
            openFileDialog.Multiselect = false;

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                caminhoModelo = openFileDialog.FileName;

                MessageBox.Show($"Arquivo selecionado: {caminhoModelo}");

                string modelo = Path.GetFileName(caminhoModelo);
                TextBoxCaminhoModelo.Text = modelo;
            }
        }

        private void BtnProximo_Click(object sender, RoutedEventArgs e)
        {
            string? nome = TextBoxNome.Text;
            string? tipo = ComboBoxTipo.Text;

            if ((!string.IsNullOrEmpty(nome.Trim())) && (!string.IsNullOrEmpty(tipo)) && (!string.IsNullOrEmpty(caminhoModelo)))
            {
                Modelo modeloNovo = new Modelo(nome, tipo, caminhoModelo);

                _gereciador.Salvar(caminhoModelo);
            }
            else
            {
                MessageBox.Show($"Preencha cada parte adequadamente");
            }

        }
    }
}
