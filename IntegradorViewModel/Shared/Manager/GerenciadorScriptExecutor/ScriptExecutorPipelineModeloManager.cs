using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ControleUsuario.ConfiguracaoTextBox;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using System.Collections.ObjectModel;
using System.Data;


namespace IntegradorViewModel.Shared.Manager.GerenciadorScriptExecutor
{
    public partial class ScriptExecutorPipelineModeloManager : ScriptExecutorManager
    {
        public ScriptExecutorPipelineModeloManager(IDialogService dialogService, IConverteJson<Dictionary<int, FuncaoDTO>> converter, IContext<ModeloDTO> contextNomeModelo, IContext<ArquivoDadosDTO> contextArquivo, IPathProvider provider, ObservableCollection<ConfiguracaoCardFuncaoViewModel> cardsFuncoes, ObservableCollection<int> opcoesPosicao, IConfiguracaoTextBox textBox) : base(dialogService, converter, contextNomeModelo, contextArquivo, provider, cardsFuncoes, opcoesPosicao, textBox) 
        {
            onConstroiPipelineAsync = ConstroiPipelineAsync;
        }

        private async Task ConstroiPipelineAsync(string caminho) => await ExecutaPipeline(caminho);
        private async Task ConstroiPipelineAsync() => await ExecutaPipeline(Path.Combine(_provider.GetCaminhoModelo(), "pipeline.json"));


        [RelayCommand]
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
                PreparaParaJson();
            }
            catch (Exception ex)
            {
                CardsFuncoes.Clear();
                OpcoesPosicao.Clear();
                _dialogService.ShowMessage($"Erro no ao carregar Pipeline: {ex.Message}", "Erro de Comando");
            }
        }

        [RelayCommand]
        public async Task AtualizaFuncao()
        {
            var modeloNomeCorpo = _textBox.MandaCodigoMetodo();

            if (modeloNomeCorpo is null)
            {
                return;
            }

            var caminhoPasta = _provider.GetCaminhoModelo();
            caminhoPasta = Path.Combine(caminhoPasta, _nomeModelo, "pipeline.json");

            var dicionarioFuncoes = _converter.CarregarJson(caminhoPasta);

            foreach (var elemento in dicionarioFuncoes)
            {
                if (elemento.Value.NomeFuncao == modeloNomeCorpo.First().Key)
                {
                    int posicao = elemento.Key;
                    var listaCodigo = modeloNomeCorpo.First().Value;

                    var funcaoDto = new FuncaoDTO(elemento.Value.NomeFuncao, listaCodigo, elemento.Value.NomeModelo);

                    var funcaoReserva = dicionarioFuncoes[posicao];
                    dicionarioFuncoes[posicao] = funcaoDto;
                    _converter.ConverteJson(dicionarioFuncoes);

                    try
                    {
                        await Task.Run(() => onConstroiPipelineAsync!());
                        _textBox.EsvaziaScript();
                    }
                    catch (Exception ex)
                    {
                        _dialogService.ShowMessage($"Houve um erro no comando: {ex.Message}", "Erro de Comando");

                        dicionarioFuncoes[posicao] = funcaoReserva;
                        _converter.ConverteJson(dicionarioFuncoes);
                        await Task.Run(() => onConstroiPipelineAsync!());
                    }

                    return;
                }
            }

            _dialogService.ShowMessage("Não há método para sobrevescrever");
        }
    }
}
