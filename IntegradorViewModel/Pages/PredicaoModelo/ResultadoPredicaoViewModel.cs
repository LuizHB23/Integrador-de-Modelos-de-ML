using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.ArquivosController.Csv;
using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.InferenciaAplicacao;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.Inferencia;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ControleUsuario.ConfiguracaoMetodo.EstadoDataFrame;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using IntegradorViewModel.Shared.Manager.GerenciadorScriptExecutor;
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

        private readonly ScriptExecutorResultadoManager _scriptManager;
        private readonly INotificationService _notificationService;
        private readonly IContext<ModeloDTO> _contextModelo;
        private readonly IDialogService _dialogService;

        private List<ErrosInferencia>? _listaErros;
        private DataFrame? _resultadosDataFrame;
        private Inferencia<SaidaDTO> _inferencia;
        private CsvController _csvController;

        public Stopwatch Stopwatch { get; set; }

        private ArquivoDadosDTO _arquivo { get; set; }

        public ResultadoPredicaoViewModel(INavigationService navigation, IDialogService dialogService, IContext<ModeloDTO> contextModelo, IContext<ArquivoDadosDTO> contextArquivo, IConverteJson<Dictionary<int, SaidaDTO>> converter, IPathProvider provider, INotificationService notificationService,Inferencia<SaidaDTO> inferencia)
        {
            Navigation = navigation;
            DataPreview = new();
            CardsFuncoes = new();
            OpcoesPosicao = new();

            _arquivo = contextArquivo.RecebeMensagem();
            _notificationService = notificationService;
            _contextModelo = contextModelo;
            _dialogService = dialogService;

            _csvController = new CsvController();
            _inferencia = inferencia;

            TextBox = new ConfiguracaoResultadoTextBoxViewModel(new ConfiguracaoTextBoxViewModel(dialogService, AlterouTabela), DataPreview, new EstadoDataFrameViewModel(_arquivo));

            _scriptManager = new(dialogService, converter, contextModelo, contextArquivo, provider, CardsFuncoes, OpcoesPosicao, TextBox);

            Stopwatch = new Stopwatch();
        }

        [RelayCommand]
        private void ExportarCsvResultado()
        {
            var caminhoArquivo = _csvController.EscreveArquivo(_resultadosDataFrame);
            LancarNotificao(caminhoArquivo);
        }

        [RelayCommand]
        private void ExportarCsvErros()
        {
            var caminhoArquivo = _csvController.EscreveArquivo(_listaErros);
            LancarNotificao(caminhoArquivo);
        }

        private void LancarNotificao(string caminhoArquivo)
        {
            _notificationService.Notify(
                "Arquivo salvo em Downloads",
                "Abrir",
                () =>
                {
                    Process.Start("explorer.exe", caminhoArquivo);
                });
        }

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

            bool erroCarregarPipeline = false;
            try
            {
                await _scriptManager.CarregarPipeline();

            }
            catch (Exception)
            {
                erroCarregarPipeline = true;
            }

            var caminhoModelo = _contextModelo.RecebeMensagem().CaminhoPasta;
            var caminhoPasta = Path.GetDirectoryName(caminhoModelo);
            var caminhoSchema = Path.Combine(caminhoPasta!, "schema.json");
            var caminhoPipeline = Path.Combine(caminhoPasta!, "pipeline.json");
            var caminhoTransformador = Path.Combine(caminhoPasta!, "transformador.json");

            var resultados = await _inferencia.RealizaInferenciaAsync(
                await CarregarDataFrameAsync(),
                caminhoModelo,
                caminhoSchema!,
                caminhoPipeline!,
                caminhoTransformador!
            );
            LinhasInferencia = resultados.Count;

            _listaErros = _inferencia.ListaErros;
            LinhasErro = _listaErros.Count;

            stopwatch.Stop();
            cts.Cancel();

            TempoProcessamento = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.ff");

            await TextBox.GuardaEstado(resultados);

            if(!erroCarregarPipeline)
            {
                var caminhoSaida = Path.Combine(caminhoPasta!, "saida.json");
                _resultadosDataFrame = await _scriptManager.CarregarPipeline(caminhoSaida);
            }
            else
            {
                TextBox.AtualizaTabela(TextBox.CarregarDados());
            }
        }

        [RelayCommand]
        public async Task AdicionaFuncao() => await _scriptManager.AdicionaFuncao();

        [RelayCommand]
        public async Task AtualizaFuncao() => await _scriptManager.AtualizaFuncao();

        public void AlterouTabela(DataView dataView) => DataPreview = dataView;

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