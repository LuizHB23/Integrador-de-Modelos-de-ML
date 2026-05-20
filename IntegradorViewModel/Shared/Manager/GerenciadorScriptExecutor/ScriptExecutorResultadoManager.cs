using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.ConversorJSON;
using IntegradorDominio.Models.DataFrameModel;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ControleUsuario.ConfiguracaoTextBox;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Factory;
using IntegradorViewModel.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace IntegradorViewModel.Shared.Manager.GerenciadorScriptExecutor
{
    public partial class ScriptExecutorResultadoManager : ScriptExecutorManager <SaidaDTO>
    {
        public ScriptExecutorResultadoManager(IDialogService dialogService, IConverteJson<Dictionary<int, SaidaDTO>> converter, IContext<ModeloDTO> contextNomeModelo, IContext<ArquivoDadosDTO> contextArquivo, IPathProvider provider, ObservableCollection<ConfiguracaoCardFuncaoViewModel> cardsFuncoes, ObservableCollection<int> opcoesPosicao, IConfiguracaoTextBox textBox) : base(dialogService, converter, contextNomeModelo, contextArquivo, provider, cardsFuncoes, opcoesPosicao, textBox) 
        {
            onConstroiPipelineAsync = ConstroiPipelineAsync;

            _json = "saida.json";
        }

        private async Task<DataFrame> ConstroiPipelineAsync(string caminho) => await ExecutaPipeline(caminho);
        private async Task ConstroiPipelineAsync() => await ExecutaPipeline(Path.Combine(_provider.GetCaminhoModelo(), _nomeModelo, _json));

        public async Task AtualizaFuncao() => await AtualizaFuncao<SaidaDTOFactory>();

        public async Task CarregarPipeline()
        {
            string caminhoPipeline = Path.Combine(_provider.GetCaminhoModelo(), _nomeModelo, _json);

            if(File.Exists(caminhoPipeline))
            {
                _cardsManager.CarregarPipeline(_converter, ConfigurarFuncao, RemoverFuncao, caminhoPipeline);
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
