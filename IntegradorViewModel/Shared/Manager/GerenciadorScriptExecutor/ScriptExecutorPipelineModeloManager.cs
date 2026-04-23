using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.DTO.Interfaces;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ControleUsuario.ConfiguracaoTextBox;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Factory;
using IntegradorViewModel.Shared.Interfaces;
using IntegradorViewModel.Shared.Manager.GerenciadorCards;
using System.Collections.ObjectModel;
using System.Data;


namespace IntegradorViewModel.Shared.Manager.GerenciadorScriptExecutor
{
    public partial class ScriptExecutorPipelineModeloManager : ScriptExecutorManager<FuncaoDTO>
    {
        public ScriptExecutorPipelineModeloManager(IDialogService dialogService, IConverteJson<Dictionary<int, FuncaoDTO>> converter, IContext<ModeloDTO> contextNomeModelo, IContext<ArquivoDadosDTO> contextArquivo, IPathProvider provider, ObservableCollection<ConfiguracaoCardFuncaoViewModel> cardsFuncoes, ObservableCollection<int> opcoesPosicao, IConfiguracaoTextBox textBox) : base(dialogService, converter, contextNomeModelo, contextArquivo, provider, cardsFuncoes, opcoesPosicao, textBox) 
        {
            onConstroiPipelineAsync = ConstroiPipelineAsync;

            _json = "pipeline.json";
        }

        private async Task ConstroiPipelineAsync(string caminho) => await ExecutaPipeline(caminho);
        private async Task ConstroiPipelineAsync() => await ExecutaPipeline(Path.Combine(_provider.GetCaminhoModelo(), _nomeModelo, _json));

        public async Task CarregarPipeline()
        {
            string? caminhoPipeline = null;

            try
            {
                caminhoPipeline = _cardsManager.CarregarPipeline(_dialogService, _converter, ConfigurarFuncao, RemoverFuncao);
            }
            catch (Exception ex)
            {
                return;
            }

            await _textBox.GuardaEstado();

            try
            {
                await ConstroiPipelineAsync(caminhoPipeline);
                PreparaParaJson<FuncaoDTOFactory>();
            }
            catch (Exception ex)
            {
                CardsFuncoes.Clear();
                OpcoesPosicao.Clear();
                _dialogService.ShowMessage($"Erro no ao carregar Pipeline: {ex.Message}", "Erro de Comando");
            }
        }

        public async Task AtualizaFuncao() => await base.AtualizaFuncao<FuncaoDTOFactory>();

        protected override void AoAlterarPipeline()
        {
            PreparaParaJson<FuncaoDTOFactory>();
        }
    }
}
