using AutoMapper;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorDominio.Models.Configuracao;
using IntegradorDominio.Models.DataFrameModel;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ControleUsuario.ConfiguracaoTextBox;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Factory;
using IntegradorViewModel.Shared.Interfaces;
using System.Collections.ObjectModel;

namespace IntegradorViewModel.Shared.Manager.GerenciadorScriptExecutor
{
    public partial class ScriptExecutorResultadoManager : ScriptExecutorManager<SaidaDTO, PipelineSaidaInferenciaConfiguracao>
    {
        public ScriptExecutorResultadoManager(IDialogService dialogService, IConversorJson conversor, IContext<ModeloDTO> contextNomeModelo, IContext<ArquivoDadosDTO> contextArquivo, IPathProvider provider, IMapper mapper, ObservableCollection<ConfiguracaoCardFuncaoViewModel> cardsFuncoes, ObservableCollection<int> opcoesPosicao, IConfiguracaoTextBox textBox) : base(dialogService, conversor, contextNomeModelo, contextArquivo, provider, mapper, cardsFuncoes, opcoesPosicao, textBox) 
        {
            _onConstroiPipelineAsync = ConstroiPipelineAsync;
        }

        private async Task<DataFrame> ConstroiPipelineAsync() => await ExecutaPipeline(await PreparaScriptCodigo());

        private async Task<DataFrame> ConstroiPipelineAsync(PipelineSaidaInferenciaConfiguracao pipeline) => await ExecutaPipeline(pipeline);

        public async Task AtualizaFuncao() => await AtualizaFuncao<SaidaDTOFactory>();

        public async Task CarregarPipeline()
        {
            string caminhoPipeline = _provider.GetCaminhoSaidaConfig(_modelo.NomeModelo);

            PipelineSaidaInferenciaConfiguracao pipeline;

            if (File.Exists(caminhoPipeline))
            {
                pipeline = (await _conversor.CarregarJsonAsync<List<PipelineSaidaInferenciaConfiguracao>>(_modelo.NomeModelo)).First();

                await _cardsManager.CarregarPipeline(ConfigurarFuncao, RemoverFuncao, pipeline.Dicionario);
            }
            else
            {
                throw new Exception("");
            }
        }

        public async Task<DataFrame> CarregarPipeline(string caminhoPipeline) => await CarregarPipeline<SaidaDTOFactory>(ConstroiPipelineAsync, await PreparaScriptCodigo());

        protected override async Task AoAlterarPipeline()
        {
            await PreparaParaJson<SaidaDTOFactory>();
        }

        private async Task<PipelineSaidaInferenciaConfiguracao> PreparaScriptCodigo() => (await _conversor.CarregarJsonAsync<List<PipelineSaidaInferenciaConfiguracao>>(_modelo.NomeModelo)).First();
    }
}
