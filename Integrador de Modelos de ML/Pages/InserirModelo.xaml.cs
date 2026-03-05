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
    }
}
