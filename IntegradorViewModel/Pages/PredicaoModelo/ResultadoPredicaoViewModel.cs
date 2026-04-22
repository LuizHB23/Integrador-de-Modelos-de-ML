using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.ArquivosController.Csv;
using IntegradorAplicacao.ArquivosController.Csv.ExportarCsv;
using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.InferenciaAplicacao;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.Inferencia;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ControleUsuario.ConfiguracaoMetodo.EstadoDataFrame;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Timers;

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

        private readonly IContext<ArquivoDadosDTO> _contextArquivo;
        private readonly IContext<ModeloDTO> _contextModelo;

        private List<ResultadoInferencia>? _resultados;
        private List<ErrosInferencia>? _listaErros;
        private Inferencia _inferencia;
        private CsvController _csvController;

        public Stopwatch Stopwatch { get; set; }

        private ArquivoDadosDTO _arquivo { get; set; }

        public ResultadoPredicaoViewModel(INavigationService navigation, IDialogService dialogService, IContext<ModeloDTO> contextModelo, IContext<ArquivoDadosDTO> contextArquivo, Inferencia inferencia)
        {
            Navigation = navigation;
            DataPreview = new();
            CardsFuncoes = new();
            OpcoesPosicao = new();

            _contextArquivo = contextArquivo;
            _contextModelo = contextModelo;

            _arquivo = contextArquivo.RecebeMensagem();

            _inferencia = inferencia;
            _csvController = new CsvController();

            TextBox = new ConfiguracaoResultadoTextBoxViewModel(new ConfiguracaoTextBoxViewModel(dialogService, _contextArquivo.RecebeMensagem(), AlterouTabela), DataPreview, new EstadoDataFrameViewModel(_arquivo));

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

            await TextBox.GuardaEstadoResultado(_resultados);
            TextBox.AtualizaTabela(TextBox.CarregarDados());
        }

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