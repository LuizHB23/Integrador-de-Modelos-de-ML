using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorAplicacao;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.DTO.Interfaces;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorDominio.Models.Configuracao;
using IntegradorDominio.Models.Configuracao.Interfaces;
using IntegradorDominio.Models.DataFrameModel;
using IntegradorDominio.Models.ModeloEtapas;
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
    public partial class ScriptExecutorManager<TIn, TOut> : 
        ObservableObject 
        where TIn : IPipelineDTO
        where TOut : class, IPipelineConfiguracao
    {
        protected readonly IConfiguracaoTextBox _textBox;
        protected readonly IDialogService _dialogService;
        protected readonly IContext<ArquivoDadosDTO> _contextArquivo;
        protected readonly IContext<ModeloDTO> _contextNomeModelo;
        protected readonly IPathProvider _provider;
        protected readonly IConversorJson _conversor;
        protected readonly IMapper _mapper;

        public ObservableCollection<int> OpcoesPosicao;
        public ObservableCollection<ConfiguracaoCardFuncaoViewModel> CardsFuncoes { get; }

        protected readonly CardsConfigurarFuncaoManager<TIn, TOut> _cardsManager;
        protected readonly ModeloDTO _modelo;

        protected ExecutorFinal<TOut>? _executor;
        protected Func<Task<DataFrame>>? _onConstroiPipelineAsync;

        public ScriptExecutorManager(IDialogService dialogService, IConversorJson conversor, IContext<ModeloDTO> contextNomeModelo, IContext<ArquivoDadosDTO> contextArquivo, IPathProvider provider, IMapper mapper, ObservableCollection<ConfiguracaoCardFuncaoViewModel> cardsFuncoes, ObservableCollection<int> opcoesPosicao, IConfiguracaoTextBox textBox)
        {
            OpcoesPosicao = opcoesPosicao;
            CardsFuncoes = cardsFuncoes;

            _dialogService = dialogService;
            _conversor = conversor;
            _contextNomeModelo = contextNomeModelo;
            _contextArquivo = contextArquivo;
            _provider = provider;
            _mapper = mapper;

            _modelo = contextNomeModelo.RecebeMensagem();
            _cardsManager = new(CardsFuncoes, OpcoesPosicao, _modelo);
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
            await AoAlterarPipeline();

            try
            {
                await _onConstroiPipelineAsync!();
                _textBox.EsvaziaScript();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Houve um erro no comando: {ex.Message}", "Erro de Comando");
            }
        }

        protected async Task<DataFrame> CarregarPipeline<F>(Func<TOut, Task<DataFrame>> onConstroiPipelineAsync, TOut objeto) where F: IPipelineExecutorFactory<TIn, TOut>
        {
            DataFrame? dataFrame = null;
            try
            {
                dataFrame = await onConstroiPipelineAsync(objeto);
                await PreparaParaJson<F>();
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
            await _cardsManager.RemoverCard(cardSchema);
            await AoAlterarPipeline();

            try
            {
                await _onConstroiPipelineAsync!();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Houve um erro no comando {ex.Message}", "Erro de Comando");
            }
        }

        protected async Task OrganizaPosicao(ConfiguracaoCardFuncaoViewModel cardSchema, int posicaoNova)
        {
            await _cardsManager.OrganizaPosicao(cardSchema, posicaoNova);
            await AoAlterarPipeline();
        }

        protected async Task PreparaParaJson<F>() where F : IPipelineExecutorFactory<TIn, TOut> => await _cardsManager.PreparaParaJson<F>(_conversor, _modelo.NomeModelo);

        protected async Task<DataFrame> ExecutaPipeline(TOut codigo)
        {
            var dataFrame = _textBox.CarregarDados();
            _executor = new();
            await _executor.ConstroiSequenciaMetodoPipeline(codigo);
            dataFrame = await Task.Run(() => _executor.ExecutarTudo(dataFrame));
            _executor = null;
            _textBox.AtualizaTabela(dataFrame);

            return dataFrame;
        }

        public async Task AtualizaFuncao<F>() where F : IPipelineExecutorFactory<TIn, TOut>
        {
            var dicionarioFuncoes = await _conversor.CarregarJsonAsync<TOut>(_modelo.NomeModelo);
            var modeloNomeCorpo = _textBox.MandaCodigoMetodo();

            if (modeloNomeCorpo is null)
            {
                return;
            }

            var elemento = dicionarioFuncoes.Dicionario.FirstOrDefault(e => e.Value.NomeFuncao == modeloNomeCorpo.First().Key);

            if(elemento.Value is null)
            {
                _dialogService.ShowMessage("Não há método para sobrevescrever");
                return;
            }

            int posicao = elemento.Key;
            var listaCodigo = modeloNomeCorpo.First().Value;

            var pipeline = F.Criar(CardsFuncoes, _modelo.NomeModelo);

            var funcaoReserva = dicionarioFuncoes.Dicionario[posicao];

            var listaPipeline = await _conversor.CarregarJsonAsync<List<TOut>>(_modelo.NomeModelo);
            listaPipeline.First(p => p.Versao == dicionarioFuncoes.Versao).Dicionario[posicao] = pipeline.First().Dicionario[posicao];

            await _conversor.ConverteJsonAsync(listaPipeline, _modelo.NomeModelo);

            try
            {
                await Task.Run(() => _onConstroiPipelineAsync!());
                _textBox.EsvaziaScript();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Houve um erro no comando: {ex.Message}", "Erro de Comando");

                listaPipeline.First(p => p.Versao == dicionarioFuncoes.Versao).Dicionario[posicao] = funcaoReserva;
                await _conversor.ConverteJsonAsync(listaPipeline, _modelo.NomeModelo);
                await Task.Run(() => _onConstroiPipelineAsync!());
            }
        }

        public async Task ConfigurarFuncao(ConfiguracaoCardFuncaoViewModel cardSchema)
        {
            Dictionary<int, Pipeline> dicionarioFuncoes;
            if (typeof(TOut) == typeof(PipelineSaidaInferenciaConfiguracao))
            {
                dicionarioFuncoes = (await _conversor.CarregarJsonAsync<List<PipelineSaidaInferenciaConfiguracao>>(_modelo.NomeModelo)).First().Dicionario;
            }
            else
            {
                dicionarioFuncoes = (await _conversor.CarregarJsonAsync<TOut>(_modelo.NomeModelo)).Dicionario;

            }
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

        protected virtual async Task AoAlterarPipeline() { }
    }
}