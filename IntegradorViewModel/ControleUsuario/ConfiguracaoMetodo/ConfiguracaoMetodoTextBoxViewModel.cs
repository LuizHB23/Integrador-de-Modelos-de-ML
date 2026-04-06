using CommunityToolkit.Mvvm.ComponentModel;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.PipelineAplicacao.ParserPipeline;
using IntegradorDominio.DataFrameModel;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace IntegradorViewModel.ControleUsuario
{
    public partial class ConfiguracaoMetodoTextBoxViewModel : ObservableObject
    {
        [ObservableProperty]
        private DataView _dadosPreview;

        [ObservableProperty]
        private string _scriptCodigo;

        [ObservableProperty]
        private bool _dataFrameMudou;

        private ParserAst _parserAst;
        private List<string>[] _estadoColuna;
        private string[] _estadoCabecalho;

        private readonly Action<DataView> _onDadosAlterados;

        private readonly ArquivoDadosDTO _arquivoDados;

        private readonly IDialogService _dialogService;

        public ConfiguracaoMetodoTextBoxViewModel(IDialogService dialogService, ArquivoDadosDTO arquivoDados, Action<DataView> onDadosAlterados, DataView dadosPreview)
        {
            ScriptCodigo = string.Empty;
            DadosPreview = dadosPreview;
            DataFrameMudou = false;

            _dialogService = dialogService;
            _arquivoDados = arquivoDados;
            _onDadosAlterados = onDadosAlterados;

            _parserAst = new();
            _estadoColuna = new List<string>[0];
            _estadoCabecalho = new string[0];

            GuardaEstado();
            AtualizaTabela(CarregarDados());
        }

        public Dictionary<string, List<string>>? MandaCodigoMetodo()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(ScriptCodigo))
                {
                    return _parserAst.ParserCorpo(ScriptCodigo);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Código do método está errado: {ex.Message}", "Código Errado");
            }

            return null;

        }

        private void GuardaEstado()
        {
            var linhas = File.ReadAllLines(_arquivoDados.CaminhoArquivoDados);
            _estadoCabecalho = ParseCsvLine(linhas[0]);

            _estadoColuna = _estadoCabecalho.Select(_ => new List<string>()).ToArray();

            for (int i = 1; i < linhas.Length; i++)
            {
                var partes = ParseCsvLine(linhas[i]);

                for (int j = 0; j < _estadoColuna.Length; j++)
                {
                    if (j < partes.Length)
                        _estadoColuna[j].Add(partes[j]);
                    else
                        _estadoColuna[j].Add(string.Empty);
                }
            }
        }

        // Função simples para parsear CSV com aspas
        private string[] ParseCsvLine(string linha)
        {
            var resultado = new List<string>();
            bool dentroAspas = false;
            var buffer = new StringBuilder();

            foreach (char c in linha)
            {
                if (c == '"')
                {
                    dentroAspas = !dentroAspas; // alterna estado
                    continue; // remove as aspas
                }

                if (c == ',' && !dentroAspas)
                {
                    resultado.Add(buffer.ToString());
                    buffer.Clear();
                }
                else
                {
                    buffer.Append(c);
                }
            }

            resultado.Add(buffer.ToString()); // adiciona último campo
            return resultado.ToArray();
        }

        public DataFrame CarregarDados()
        {
            var dataFrame = new DataFrame();

            for (int i = 0; i < _estadoCabecalho.Length; i++)
            {
                var dado = _estadoColuna[i];
                dataFrame.AdicionarColuna(_estadoCabecalho[i], dado);
            }

            AtualizaTabela(dataFrame);

            return dataFrame;
        }

        public static DataTable DataFrameParaDataTable(DataFrame dataFrame)
        {
            var tabela = new DataTable();
            var colunas = dataFrame.Colunas;
            var quantidadeColunas = colunas.Count;
            var quantidadeLinhas = dataFrame.QuantidadeLinhas;

            // Adicionar colunas ao DataTable
            for (int j = 0; j < quantidadeColunas; j++)
            {
                tabela.Columns.Add(colunas[j].Nome, typeof(object));
            }

            // Criar a primeira linha com TIPAGEM
            var linhaTipagem = tabela.NewRow();
            for (int j = 0; j < quantidadeColunas; j++)
            {
                Type tipo = colunas[j].TipoDado;

                // Mapear nullable para tipo “base” que você quer mostrar
                string tipoExibicao = tipo switch
                {
                    Type t when t == typeof(float) || t == typeof(float?) => "Single",
                    Type t when t == typeof(string) => "String",
                    Type t when t == typeof(bool) || t == typeof(bool?) => "Boolean",
                    Type t when t == typeof(DateTime) || t == typeof(DateTime?) => "DateTime",
                    _ => "Object"
                };

                linhaTipagem[j] = tipoExibicao;
            }
            tabela.Rows.Add(linhaTipagem);

            // Adicionar dados do DataFrame
            for (int i = 0; i < quantidadeLinhas; i++)
            {
                var linhaDados = tabela.NewRow();

                for (int j = 0; j < quantidadeColunas; j++)
                {
                    linhaDados[j] = colunas[j].PegarValor(i) ?? DBNull.Value;
                }

                tabela.Rows.Add(linhaDados);
            }

            return tabela;
        }

        public void AtualizaTabela(DataFrame dataFrame)
        {
            var dataTable = DataFrameParaDataTable(dataFrame);

            _onDadosAlterados(dataTable.DefaultView);
        }
    }
}