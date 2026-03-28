using CommunityToolkit.Mvvm.ComponentModel;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.PipelineAplicacao.ParserPipeline;
using IntegradorDominio.DataFrameModel;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
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

        private ParserAst _parserAst;
        private DataFrame _dataFrame;

        private readonly Action<DataView> _onDadosAlterados;

        private readonly ArquivoDadosDTO _arquivoDados;

        private readonly IDialogService _dialogService;

        public ConfiguracaoMetodoTextBoxViewModel(IDialogService dialogService, ArquivoDadosDTO arquivoDados, Action<DataView> onDadosAlterados, DataView dadosPreview)
        {
            ScriptCodigo = string.Empty;
            DadosPreview = dadosPreview;

            _parserAst = new ParserAst();
            _dialogService = dialogService;
            _arquivoDados = arquivoDados;
            _onDadosAlterados = onDadosAlterados;

            _dataFrame = CarregarDados();
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

        public DataFrame CarregarDados()
        {
            var linhas = File.ReadAllLines(_arquivoDados.CaminhoArquivoDados);
            var cabecalho = linhas[0].Split(',');

            var colunas = cabecalho.Select(_ => new List<string>()).ToArray();

            for (int i = 1; i < 22; i++)
            {
                var partes = linhas[i].Split(_arquivoDados.Delimitador);
                for (int j = 0; j < partes.Length; j++)
                    colunas[j].Add(partes[j]);
            }

            var dataFrame = new DataFrame();

            for (int i = 0; i < cabecalho.Length; i++)
            {
                var dado = colunas[i].ToArray();
                dataFrame.AddColumn(cabecalho[i], dado);
            }

            var dataTable = DataFrameParaDataTable(dataFrame);

            _onDadosAlterados(dataTable.DefaultView);

            return dataFrame;
        }

        public static DataTable DataFrameParaDataTable(DataFrame dataFrame)
        {
            var tabela = new DataTable();

            var colunas = dataFrame.Colunas;
            var quantidadeColunas = colunas.Count;

            for (int j = 0; j < quantidadeColunas; j++)
            {
                tabela.Columns.Add(colunas[j].Nome, colunas[j].TipoDado);
            }

            var quantidadeLinhas = dataFrame.QuantidadeLinhas;

            for (int i = 0; i < quantidadeLinhas; i++)
            {
                var linha = tabela.NewRow();

                for (int j = 0; j < quantidadeColunas; j++)
                {
                    linha[j] = colunas[j].GetValue(i) ?? DBNull.Value;
                }

                tabela.Rows.Add(linha);
            }

            return tabela;
        }
    }
}