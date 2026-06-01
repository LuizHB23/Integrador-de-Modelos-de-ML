using AutoMapper;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorDominio.Models.Configuracao;
using IntegradorDominio.Models.DataFrameModel;
using IntegradorDominio.Models.ModeloEtapas;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ControleUsuario.ConfiguracaoTextBox;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Factory;
using IntegradorViewModel.Shared.Interfaces;
using System.Collections.ObjectModel;

namespace IntegradorViewModel.Shared.Manager.GerenciadorScriptExecutor
{
    public partial class ScriptExecutorPipelineModeloManager : ScriptExecutorManager<FuncaoDTO, PipelineTratamentoConfiguracao>
    {
        public ScriptExecutorPipelineModeloManager(IDialogService dialogService, IConversorJson conversor, IContext<ModeloDTO> contextNomeModelo, IContext<ArquivoDadosDTO> contextArquivo, IPathProvider provider, IMapper mapper, ObservableCollection<ConfiguracaoCardFuncaoViewModel> cardsFuncoes, ObservableCollection<int> opcoesPosicao, IConfiguracaoTextBox textBox) : base(dialogService, conversor, contextNomeModelo, contextArquivo, provider, mapper, cardsFuncoes, opcoesPosicao, textBox) 
        {
            _onConstroiPipelineAsync = ConstroiPipelineAsync;
        }

        private async Task<DataFrame> ConstroiPipelineAsync() => await ExecutaPipeline(await PreparaScriptCodigo());
        private async Task<DataFrame> ConstroiPipelineAsync(PipelineTratamentoConfiguracao pipeline) => await ExecutaPipeline(pipeline);

        public async Task CarregarPipeline()
        {
            string? caminhoPipeline = null;

            PipelineTratamentoConfiguracao pipelineTratamentoConfiguracao;

            try
            {
                caminhoPipeline = _dialogService.GetCaminhoArquivo();

                if (string.IsNullOrWhiteSpace(caminhoPipeline))
                {
                    throw new Exception();
                }


                var pipeline = await _conversor.CarregarJsonAsync<Dictionary<int, FuncaoDTO>>(caminhoPipeline);

                var pipelineMapeado = _mapper.Map<Dictionary<int, Pipeline>>(pipeline);

                var nomeModelo = _contextNomeModelo.RecebeMensagem().NomeModelo;

                pipelineTratamentoConfiguracao = new(nomeModelo, "1.0", pipelineMapeado);

                await _cardsManager.CarregarPipeline(ConfigurarFuncao, RemoverFuncao, pipelineMapeado);

                await _conversor.ConverteJsonAsync(pipelineTratamentoConfiguracao, nomeModelo);
            }
            catch (Exception)
            {
                return;
            }

            await CarregarPipeline<FuncaoDTOFactory>(ConstroiPipelineAsync, pipelineTratamentoConfiguracao);

        }

        public async Task AtualizaFuncao() => await AtualizaFuncao<FuncaoDTOFactory>();

        protected override async Task AoAlterarPipeline() => await PreparaParaJson<FuncaoDTOFactory>();

        private async Task<PipelineTratamentoConfiguracao> PreparaScriptCodigo() => await _conversor.CarregarJsonAsync<PipelineTratamentoConfiguracao>(_modelo.NomeModelo);

    }
}
