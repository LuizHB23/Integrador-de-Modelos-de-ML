using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.ArquivosController.Csv;
using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.InferenciaAplicacao;
using IntegradorAplicacao.PipelineAplicacao.ExecutorAplicacao;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using IntegradorDominio.Inferencia;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ControleUsuario.ConfiguracaoMetodo.EstadoDataFrame;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using IntegradorViewModel.Shared.Manager.GerenciadorCards;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;

namespace IntegradorViewModel.Pages.PredicaoModelo
{
    public partial class ResultadoPredicaoViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _tempoProcessamento = "00:00:00.00";

        [ObservableProperty]
        private int _linhasInferencia = 0;

        [ObservableProperty]
        private int _linhasErro = 0;

        [ObservableProperty]
        private DataView? _dataPreview;

        [ObservableProperty]
        private ConfiguracaoResultadoTextBoxViewModel _textBox;

        [ObservableProperty]
        private INavigationService _navigation;

        public ObservableCollection<int> OpcoesPosicao;
        public ObservableCollection<ConfiguracaoCardFuncaoViewModel> CardsFuncoes { get; }

        private readonly IConverteJson<Dictionary<int, FuncaoDTO>> _converter;
        private readonly IContext<ArquivoDadosDTO> _contextArquivo;
        private readonly IContext<ModeloDTO> _contextModelo;
        private readonly IDialogService _dialogService;
        private readonly IPathProvider _provider;

        private readonly CardsConfigurarFuncaoManager _cardsManager;
        private readonly string _nomeModelo;

        private List<ResultadoInferencia>? _resultados;
        private List<ErrosInferencia>? _listaErros;
        private CsvController _csvController;
        private ExecutorFinal? _executor;
        private Inferencia _inferencia;

        public Stopwatch Stopwatch { get; set; }

        private ArquivoDadosDTO _arquivo { get; set; }

        public ResultadoPredicaoViewModel(INavigationService navigation, IDialogService dialogService, IContext<ModeloDTO> contextModelo, IContext<ArquivoDadosDTO> contextArquivo, IConverteJson<Dictionary<int, FuncaoDTO>> converter, IPathProvider provider, Inferencia inferencia)
        {
            Navigation = navigation;
            DataPreview = new();
            CardsFuncoes = new();
            OpcoesPosicao = new();

            _contextArquivo = contextArquivo;
            _dialogService = dialogService;
            _contextModelo = contextModelo;
            _provider = provider;
            _converter = converter;

            _arquivo = contextArquivo.RecebeMensagem();

            _nomeModelo = _contextModelo.RecebeMensagem().NomeModelo;
            _cardsManager = new(CardsFuncoes, OpcoesPosicao);

            _inferencia = inferencia;
            _csvController = new CsvController();

            TextBox = new ConfiguracaoResultadoTextBoxViewModel(new ConfiguracaoTextBoxViewModel(dialogService, AlterouTabela), DataPreview, new EstadoDataFrameViewModel(_arquivo));

            Stopwatch = new Stopwatch();
        }

        [RelayCommand]
        private void ExportarCsvResultado() => _csvController.EscreveArquivo(_resultados);

        [RelayCommand]
        private void ExportarCsvErros() => _csvController.EscreveArquivo(_listaErros);

        public async Task InicializarAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            var cts = new CancellationTokenSource();

            var token = cts.Token;

            var timerTask = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    TempoProcessamento = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.ff");
                    await Task.Delay(50, token);
                }
            }, token);

            var caminhoModelo = _contextModelo.RecebeMensagem().CaminhoPasta;
            var caminhoPasta = Path.GetDirectoryName(caminhoModelo);
            var caminhoSchema = Path.Combine(caminhoPasta!, "schema.json");
            var caminhoPipeline = Path.Combine(caminhoPasta!, "pipeline.json");
            var caminhoTransformadores = Path.Combine(caminhoPasta!, "transformador.json");

            _resultados = await _inferencia.RealizaInferenciaAsync(
                await CarregarDataFrameAsync(),
                caminhoModelo,
                caminhoSchema,
                caminhoPipeline,
                caminhoTransformadores
            );
            LinhasInferencia = _resultados.Count;

            _listaErros = _inferencia.ListaErros;
            LinhasErro = _listaErros.Count;

            stopwatch.Stop();
            cts.Cancel();

            TempoProcessamento = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.ff");

            //await TextBox.GuardaEstadoResultado(_resultados);
            TextBox.AtualizaTabela(TextBox.CarregarDados());
        }

        [RelayCommand]
        public async Task AdicionaFuncao()
        {
            var modeloNomeCorpo = TextBox.MandaCodigoMetodo();

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
                await ConstroiPipelineAsync();
                TextBox.EsvaziaScript();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Houve um erro no comando: {ex.Message}", "Erro de Comando");
            }
        }

        private async Task RemoverFuncao(ConfiguracaoCardFuncaoViewModel cardSchema)
        {
            _cardsManager.RemoverCard(cardSchema);
            PreparaParaJson();

            try
            {
                await ConstroiPipelineAsync();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Houve um erro no comando {ex.Message}", "Erro de Comando");
            }
        }

        private void OrganizaPosicao(ConfiguracaoCardFuncaoViewModel cardSchema, int posicaoNova)
        {
            _cardsManager.OrganizaPosicao(cardSchema, posicaoNova);
            PreparaParaJson();
        }

        private void PreparaParaJson() => _cardsManager.PreparaParaJson(_converter, _nomeModelo);

        public void AlterouTabela(DataView dataView) => DataPreview = dataView;

        private async Task ConstroiPipelineAsync() => await ExecutaPipeline(Path.Combine(_provider.GetCaminhoModelo(), "pipeline.json"));

        private async Task ExecutaPipeline(string caminho)
        {
            var dataFrame = TextBox.CarregarDados();
            _executor = new(_converter);
            await Task.Run(() => _executor.ConstroiSequenciaMetodoPipeline(caminho));
            dataFrame = await Task.Run(() => _executor.ExecutarTudo(dataFrame));
            _executor = null;
            TextBox.AtualizaTabela(dataFrame);
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

            TextBox.ScriptCodigo = codigo;
        }

        private async Task<DataFrame> CarregarDataFrameAsync()
        {
            await Task.Run(() => _csvController.CarregarArquivoAsync(_arquivo.CaminhoArquivoDados));

            var cabecalho = _csvController.Cabecalho;
            var colunas = _csvController.Colunas;

            var dataFrame = new DataFrame();

            for (int i = 0; i < cabecalho.Length; i++)
            {
                dataFrame.AdicionarColuna(cabecalho[i], colunas[i]);
            }

            return dataFrame;
        }
    }
}