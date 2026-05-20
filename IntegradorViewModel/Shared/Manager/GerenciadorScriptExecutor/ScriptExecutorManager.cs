using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorAplicacao;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.DTO.Interfaces;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.ConversorJSON;
using IntegradorDominio.Models.DataFrameModel;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ControleUsuario.ConfiguracaoTextBox;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Factory;
using IntegradorViewModel.Shared.Interfaces;
using IntegradorViewModel.Shared.Manager.GerenciadorCards;
using System.Collections.ObjectModel;

namespace IntegradorViewModel.Shared.Manager.GerenciadorScriptExecutor
{
    public partial class ScriptExecutorManager<T> : ObservableObject where T : IPipelineExecutor
    {
        protected readonly IConfiguracaoTextBox _textBox;
        protected readonly IConverteJson<Dictionary<int, T>> _converter;
        protected readonly IDialogService _dialogService;
        protected readonly IContext<ArquivoDadosDTO> _contextArquivo;
        protected readonly IContext<ModeloDTO> _contextNomeModelo;
        protected readonly IPathProvider _provider;

        public ObservableCollection<int> OpcoesPosicao;
        public ObservableCollection<ConfiguracaoCardFuncaoViewModel> CardsFuncoes { get; }

        protected readonly CardsConfigurarFuncaoManager<T> _cardsManager;
        protected readonly string _nomeModelo;

        protected ExecutorFinal<T>? _executor;
        protected Func<Task>? onConstroiPipelineAsync;
        protected string _json;

        public ScriptExecutorManager(IDialogService dialogService, IConverteJson<Dictionary<int, T>> converter, IContext<ModeloDTO> contextNomeModelo, IContext<ArquivoDadosDTO> contextArquivo, IPathProvider provider, ObservableCollection<ConfiguracaoCardFuncaoViewModel> cardsFuncoes, ObservableCollection<int> opcoesPosicao, IConfiguracaoTextBox textBox)
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

            foreach(var funcao in CardsFuncoes)
            {
                if(modeloElementos.Key == funcao.NomeMetodo)
                {
                    _dialogService.ShowMessage($"Não é possível atribuir um nome de funçao já existente");
                    return;
                } 
            }

            var funcaoItem = new FuncaoItemViewModel(CardsFuncoes.Count + 1, modeloElementos.Key, modeloElementos.Value);
            _cardsManager.AdicionarCard(funcaoItem, RemoverFuncao, OrganizaPosicao, ConfigurarFuncao);
            AoAlterarPipeline();

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

        protected async Task<DataFrame> CarregarPipeline<F>(Func<string, Task<DataFrame>> funcConstroiPipeline, string caminhoPipeline) where F: IPipelineExecutorFactory<T>
        {
            DataFrame? dataFrame = null;
            try
            {
                dataFrame = await funcConstroiPipeline(caminhoPipeline);
                PreparaParaJson<F>();
            }
            catch (Exception ex)
            {
                CardsFuncoes.Clear();
                OpcoesPosicao.Clear();
                _dialogService.ShowMessage($"Erro no ao carregar Pipeline: {ex.Message}", "Erro de Comando");
            }

            return dataFrame;
        }

        protected async Task RemoverFuncao(ConfiguracaoCardFuncaoViewModel cardSchema)
        {
            _cardsManager.RemoverCard(cardSchema);
            AoAlterarPipeline();

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
            AoAlterarPipeline();
        }

        protected void PreparaParaJson<F>() where F : IPipelineExecutorFactory<T> => _cardsManager.PreparaParaJson<F>(_converter, _nomeModelo);

        protected async Task<DataFrame> ExecutaPipeline(string caminho)
        {
            var dataFrame = _textBox.CarregarDados();
            _executor = new(_converter);
            _executor.ConstroiSequenciaMetodoPipeline(caminho);
            dataFrame = await Task.Run(() => _executor.ExecutarTudo(dataFrame));
            _executor = null;
            _textBox.AtualizaTabela(dataFrame);

            return dataFrame;
        }


        public async Task AtualizaFuncao<F>() where F : IPipelineExecutorFactory<T>
        {
            var modeloNomeCorpo = _textBox.MandaCodigoMetodo();

            if (modeloNomeCorpo is null)
            {
                return;
            }

            var caminhoPasta = _provider.GetCaminhoModelo();
            caminhoPasta = Path.Combine(caminhoPasta, _nomeModelo, _json);

            var dicionarioFuncoes = _converter.CarregarJson(caminhoPasta);

            foreach (var elemento in dicionarioFuncoes)
            {
                if (elemento.Value.NomeFuncao == modeloNomeCorpo.First().Key)
                {
                    int posicao = elemento.Key;
                    var listaCodigo = modeloNomeCorpo.First().Value;

                    var pipelineDto = F.Criar(elemento.Value.NomeFuncao, listaCodigo, elemento.Value.NomeModelo);

                    var funcaoReserva = dicionarioFuncoes[posicao];
                    dicionarioFuncoes[posicao] = pipelineDto;
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


        public void ConfigurarFuncao(ConfiguracaoCardFuncaoViewModel cardSchema)
        {
            var caminhoPasta = _provider.GetCaminhoModelo();
            caminhoPasta = Path.Combine(caminhoPasta, _nomeModelo, _json);

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

        protected virtual void AoAlterarPipeline() { }
    }
}