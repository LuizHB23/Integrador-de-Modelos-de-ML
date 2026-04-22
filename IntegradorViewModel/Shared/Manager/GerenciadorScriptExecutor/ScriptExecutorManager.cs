using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.PipelineAplicacao.ExecutorAplicacao;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ControleUsuario.ConfiguracaoTextBox;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using IntegradorViewModel.Shared.Manager.GerenciadorCards;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;

namespace IntegradorViewModel.Shared.Manager.GerenciadorScriptExecutor
{
    public partial class ScriptExecutorManager : ObservableObject 
    {
        protected readonly IConfiguracaoTextBox _textBox;
        protected readonly IConverteJson<Dictionary<int, FuncaoDTO>> _converter;
        protected readonly IDialogService _dialogService;
        protected readonly IContext<ArquivoDadosDTO> _contextArquivo;
        protected readonly IContext<ModeloDTO> _contextNomeModelo;
        protected readonly IPathProvider _provider;

        public ObservableCollection<int> OpcoesPosicao;
        public ObservableCollection<ConfiguracaoCardFuncaoViewModel> CardsFuncoes { get; }

        protected readonly CardsConfigurarFuncaoManager _cardsManager;
        protected readonly string _nomeModelo;
        protected ExecutorFinal? _executor;

        protected Func<Task>? onConstroiPipelineAsync;

        public ScriptExecutorManager(IDialogService dialogService, IConverteJson<Dictionary<int, FuncaoDTO>> converter, IContext<ModeloDTO> contextNomeModelo, IContext<ArquivoDadosDTO> contextArquivo, IPathProvider provider, ObservableCollection<ConfiguracaoCardFuncaoViewModel> cardsFuncoes, ObservableCollection<int> opcoesPosicao, IConfiguracaoTextBox textBox)
        {
            OpcoesPosicao = opcoesPosicao;
            CardsFuncoes = cardsFuncoes;

            _dialogService = dialogService;
            _converter = converter;
            _contextNomeModelo = contextNomeModelo;
            _contextArquivo = contextArquivo;
            _provider = provider;

            _nomeModelo = contextNomeModelo.RecebeMensagem().NomeModelo;
            _cardsManager = new(CardsFuncoes, OpcoesPosicao);
            _textBox = textBox;
        }

        [RelayCommand]
        public async Task AdicionaFuncao()
        {
            var modeloNomeCorpo = _textBox.MandaCodigoMetodo();

            if ((modeloNomeCorpo is null) || (modeloNomeCorpo.Count == 0))
            {
                return;
            }

            var modeloElementos = modeloNomeCorpo.First();
            var funcaoItem = new FuncaoItemViewModel(CardsFuncoes.Count + 1, modeloElementos.Key, modeloElementos.Value);
            _cardsManager.AdicionarCard(funcaoItem, RemoverFuncao, OrganizaPosicao, ConfigurarFuncao);
            PreparaParaJson();

            try
            {
                await onConstroiPipelineAsync!();
                _textBox.EsvaziaScript();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Houve um erro no comando: {ex.Message}", "Erro de Comando");
            }
        }

        protected async Task RemoverFuncao(ConfiguracaoCardFuncaoViewModel cardSchema)
        {
            _cardsManager.RemoverCard(cardSchema);
            PreparaParaJson();

            try
            {
                await onConstroiPipelineAsync!();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Houve um erro no comando {ex.Message}", "Erro de Comando");
            }
        }

        protected void OrganizaPosicao(ConfiguracaoCardFuncaoViewModel cardSchema, int posicaoNova)
        {
            _cardsManager.OrganizaPosicao(cardSchema, posicaoNova);
            PreparaParaJson();
        }

        protected void PreparaParaJson() => _cardsManager.PreparaParaJson(_converter, _nomeModelo);

        protected async Task ExecutaPipeline(string caminho)
        {
            var dataFrame = _textBox.CarregarDados();
            _executor = new(_converter);
            await Task.Run(() => _executor.ConstroiSequenciaMetodoPipeline(caminho));
            dataFrame = await Task.Run(() => _executor.ExecutarTudo(dataFrame));
            _executor = null;
            _textBox.AtualizaTabela(dataFrame);
        }

        public void ConfigurarFuncao(ConfiguracaoCardFuncaoViewModel cardSchema)
        {
            var caminhoPasta = _provider.GetCaminhoModelo();
            caminhoPasta = Path.Combine(caminhoPasta, _nomeModelo, "pipeline.json");

            var dicionarioFuncoes = _converter.CarregarJson(caminhoPasta);
            var codigo = string.Empty;

            foreach (var elemento in dicionarioFuncoes)
            {
                if (elemento.Value.NomeFuncao == cardSchema.NomeMetodo)
                {
                    codigo = $"{cardSchema.NomeMetodo}()" + "\n{";

                    foreach (var linha in elemento.Value.Codigo)
                    {

                        codigo += $"\n{linha}\n";
                    }
                    codigo += "}";
                }
            }

            _textBox.ScriptCodigo = codigo;
        }
    }
}
