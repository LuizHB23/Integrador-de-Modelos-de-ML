using IntegradorView.Pages.PrincipalModelo;
using IntegradorViewModel.Pages.InserirModelo;
using System.Windows;
using System.Windows.Controls;

namespace IntegradorView.Pages.InserirModelo
{
    /// <summary>
    /// Lógica interna para PipelineModelo.xaml
    /// </summary>
    public partial class PipelineModelo : UserControl
    {
        public PipelineModelo()
        {
            InitializeComponent();
            DataContext = App.GetService<PipelineModeloViewModel>();
        }
        public List<PipelineStep> Steps { get; set; } = new List<PipelineStep>
        {
            new PipelineStep
            {
                Hint = "Data Cleaning (Tabular/NLP)",
                Options = new List<string>
                {
                    "Remover duplicados",
                    "Corrigir tipos de dados",
                    "Remover colunas irrelevantes",
                    "Padronizar unidades",
                    "NLP: Lowercase",
                    "NLP: Remover pontuação/acentos"
                }
            },
            new PipelineStep
            {
                Hint = "Missing Values",
                Options = new List<string>
                {
                    "Média / Mediana / Moda",
                    "Valor fixo",
                    "Forward / Backward fill",
                    "NLP: Substituir por token especial"
                }
            },
        };
    }

    public class PipelineStep
    {
        public string? Hint { get; set; }                // Ex.: "Data Cleaning"
        public List<string>? Options { get; set; }       // Opções que podem ser aplicadas
        public string? SelectedOption { get; set; }      // A opção escolhida pelo usuário
    }
}
