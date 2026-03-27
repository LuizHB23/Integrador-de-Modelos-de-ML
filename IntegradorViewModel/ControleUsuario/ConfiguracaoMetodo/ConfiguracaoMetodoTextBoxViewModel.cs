using CommunityToolkit.Mvvm.ComponentModel;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.PipelineAplicacao.ParserPipeline;
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

            //CarregarDados();
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
        public void CarregarDados()
        {
            DataTable dt = new DataTable();

            using (var reader = new System.IO.StreamReader(_arquivoDados.CaminhoArquivoDados))
            {
                string[] cabecalho = reader.ReadLine()?.Split(_arquivoDados.Delimitador)!;
                foreach (var col in cabecalho!) dt.Columns.Add(col);

                for (int i = 0; i < 20; i++)
                {
                    string[] linha = reader.ReadLine()?.Split(_arquivoDados.Delimitador)!;
                    dt.Rows.Add(linha!);
                }
            }

            _onDadosAlterados(dt.DefaultView);
        }
    }
}