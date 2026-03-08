using InetradorAplicacao.DTO;
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
using System.Windows.Shapes;

namespace Integrador_de_Modelos_de_ML.Pages
{
    /// <summary>
    /// Lógica interna para PipelineModelo.xaml
    /// </summary>
    public partial class PipelineModelo : Page
    {
        public PipelineModelo()
        {
            InitializeComponent();
            this.DataContext = this;
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
