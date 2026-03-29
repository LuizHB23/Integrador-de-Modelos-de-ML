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

        [ObservableProperty]
        private bool _dataFrameMudou;

        private ParserAst _parserAst;
        private DataFrame _dataFrame;

        private readonly Action<DataView> _onDadosAlterados;

        private readonly ArquivoDadosDTO _arquivoDados;

        private readonly IDialogService _dialogService;

        public ConfiguracaoMetodoTextBoxViewModel(IDialogService dialogService, ArquivoDadosDTO arquivoDados, Action<DataView> onDadosAlterados, DataView dadosPreview, DataFrame dataFrame)
        {
            ScriptCodigo = string.Empty;
            DadosPreview = dadosPreview;
            DataFrameMudou = false;

            _parserAst = new ParserAst();
            _dialogService = dialogService;
            _arquivoDados = arquivoDados;
            _onDadosAlterados = onDadosAlterados;

            _dataFrame = dataFrame;
            CarregarDados(_dataFrame);
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

        partial void OnDataFrameMudouChanged(bool value)
        {
            if(value)
            {
                DataFrameParaDataTable(_dataFrame);
            }

            DataFrameMudou = false;
        }

        public void CarregarDados(DataFrame dataFrame)
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

            for (int i = 0; i < cabecalho.Length; i++)
            {
                var dado = colunas[i].ToArray();
                dataFrame.AdiconarColuna(cabecalho[i], dado);
            }

            var dataTable = DataFrameParaDataTable(dataFrame);

            _onDadosAlterados(dataTable.DefaultView);
        }

        public static DataTable DataFrameParaDataTable(DataFrame dataFrame)
        {
            var tabela = new DataTable();
            var colunas = dataFrame.Colunas;
            var quantidadeColunas = colunas.Count;
            var quantidadeLinhas = dataFrame.QuantidadeLinhas;

            for (int j = 0; j < quantidadeColunas; j++)
            {
                tabela.Columns.Add(colunas[j].Nome, typeof(object));
            }

            // Criar a primeira linha com as informações de TIPAGEM
            var linhaTipagem = tabela.NewRow();
            for (int j = 0; j < quantidadeColunas; j++)
            {
                linhaTipagem[j] = colunas[j].TipoDado.Name;
            }
            tabela.Rows.Add(linhaTipagem);

            // Adicionar o restante dos dados do DataFrame
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
    }
}