using CommunityToolkit.Mvvm.ComponentModel;
using IntegradorAplicacao.PipelineAplicacao.ParserPipeline;
using IntegradorViewModel.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorViewModel.ControleUsuario
{
    public partial class ConfiguracaoMetodoTextBoxViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _scriptCodigo;

        private ParserAst _parserAst;

        private readonly IDialogService _dialogService;

        public ConfiguracaoMetodoTextBoxViewModel(IDialogService dialogService)
        {
            ScriptCodigo = string.Empty;

            _parserAst = new ParserAst();
            _dialogService = dialogService;
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
    }
}