using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.DTO.Interfaces;
using IntegradorDominio.DataFrameModel;
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

        private async Task<DataFrame> ConstroiPipelineAsync(string caminho) => await ExecutaPipeline(caminho);
        private async Task ConstroiPipelineAsync() => await ExecutaPipeline(Path.Combine(_provider.GetCaminhoModelo(), _nomeModelo, _json));

        public async Task CarregarPipeline()
        {
            string? caminhoPipeline = null;

            try
            {
                caminhoPipeline = _dialogService.GetCaminhoArquivo();

                if (string.IsNullOrWhiteSpace(caminhoPipeline))
                {
                    throw new Exception();
                }

                _cardsManager.CarregarPipeline(_converter, ConfigurarFuncao, RemoverFuncao, caminhoPipeline);
            }
            catch (Exception)
            {
                return;
            }

            await CarregarPipeline<FuncaoDTOFactory>(ConstroiPipelineAsync, caminhoPipeline);
        }

        public async Task AtualizaFuncao() => await AtualizaFuncao<FuncaoDTOFactory>();

        protected override void AoAlterarPipeline()
        {
            PreparaParaJson<FuncaoDTOFactory>();
        }
    }
}
