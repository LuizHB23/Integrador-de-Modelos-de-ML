using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorDominio.Models.DataFrameModel;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ControleUsuario.ConfiguracaoTextBox;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Factory;
using IntegradorViewModel.Shared.Interfaces;
using System.Collections.ObjectModel;

namespace IntegradorViewModel.Shared.Manager.GerenciadorScriptExecutor
{
    public partial class ScriptExecutorResultadoManager : ScriptExecutorManager<SaidaDTO>
    {
        public ScriptExecutorResultadoManager(IDialogService dialogService, IConversorJson conversor, IContext<ModeloDTO> contextNomeModelo, IContext<ArquivoDadosDTO> contextArquivo, IPathProvider provider, ObservableCollection<ConfiguracaoCardFuncaoViewModel> cardsFuncoes, ObservableCollection<int> opcoesPosicao, IConfiguracaoTextBox textBox) : base(dialogService, conversor, contextNomeModelo, contextArquivo, provider, cardsFuncoes, opcoesPosicao, textBox) 
        {
            onConstroiPipelineAsync = ConstroiPipelineAsync;

            _json = "saida.json";
        }

        private async Task<DataFrame> ConstroiPipelineAsync(string caminho) => await ExecutaPipeline(caminho);
        private async Task ConstroiPipelineAsync() => await ExecutaPipeline(_provider.GetCaminhoSaidaConfig(_nomeModelo));

        public async Task AtualizaFuncao() => await AtualizaFuncao<SaidaDTOFactory>();

        public async Task CarregarPipeline()
        {
            string caminhoPipeline = _provider.GetCaminhoSaidaConfig(_nomeModelo);

            if(File.Exists(caminhoPipeline))
            {
                _cardsManager.CarregarPipeline(_conversor, ConfigurarFuncao, RemoverFuncao, caminhoPipeline);
            }
            else
            {
                throw new Exception("");
            }
        }

        public async Task<DataFrame> CarregarPipeline(string caminhoPipeline) => await CarregarPipeline<SaidaDTOFactory>(ConstroiPipelineAsync, caminhoPipeline);

        protected override void AoAlterarPipeline()
        {
            PreparaParaJson<SaidaDTOFactory>();
        }
    }
}
