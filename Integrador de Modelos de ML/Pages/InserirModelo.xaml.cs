using InetradorAplicacao.DTO;
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

                string modelo = Path.GetFileName(caminhoModelo);
                TextBoxCaminhoModelo.Text = modelo;
            }
        }

        private void BtnProximo_Click(object sender, RoutedEventArgs e)
        {
            string? nome = TextBoxNome.Text;
            string? tipo = ComboBoxTipo.Text;

            // Configuração colunas não existe mais, está para ser feito nesta mesma pagina
            // Mudando para PipelineModelo

            if ((!string.IsNullOrEmpty(nome.Trim())) && (!string.IsNullOrEmpty(tipo)) && (!string.IsNullOrEmpty(caminhoModelo)))
            {
                var caminhoDestino = _gereciador.Salvar(caminhoModelo);
                ModeloDTO modeloNovo = new ModeloDTO(nome, tipo, caminhoDestino);
                InserirFrame.Navigate(new PipelineModelo());
            }
            else
            {
                MessageBox.Show($"Preencha cada parte adequadamente");
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            InserirFrame.Navigate(new Home());
        }
    }
}
