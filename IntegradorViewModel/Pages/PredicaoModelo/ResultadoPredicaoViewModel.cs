using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.Aplicacao.InferenciaAplicacao;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.ArquivosController.Csv;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorDominio.Models.Configuracao;
using IntegradorDominio.Models.DataFrameModel;
using IntegradorDominio.Models.Inferencia;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ControleUsuario.ConfiguracaoMetodo.EstadoDataFrame;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.InserirModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using IntegradorViewModel.Shared.Manager.GerenciadorScriptExecutor;
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

        private readonly ScriptExecutorResultadoManager _scriptManager;
        private readonly INotificationService _notificationService;
        private readonly IContext<ModeloDTO> _contextModelo;
        private readonly IConversorJson _conversor;
        private readonly IPathProvider _provider;

        private List<ErrosInferencia>? _listaErros;
        private DataFrame? _resultadosDataFrame;
        private Inferencia<PipelineTratamentoConfiguracao> _inferencia;
        private CsvController _csvController;

        public Stopwatch Stopwatch { get; set; }

        private ArquivoDadosDTO _arquivo { get; set; }

        public ResultadoPredicaoViewModel(INavigationService navigation, IDialogService dialogService, IContext<ModeloDTO> contextModelo, IContext<ArquivoDadosDTO> contextArquivo, IConversorJson conversor, IPathProvider provider, INotificationService notificationService, IMapper mapper, Inferencia<PipelineTratamentoConfiguracao> inferencia)
        {
            Navigation = navigation;
            DataPreview = new();
            CardsFuncoes = new();
            OpcoesPosicao = new();

            _arquivo = contextArquivo.RecebeMensagem();
            _notificationService = notificationService;
            _contextModelo = contextModelo;
            _conversor = conversor;
            _provider = provider;

            _csvController = new CsvController();
            _inferencia = inferencia;

            TextBox = new ConfiguracaoResultadoTextBoxViewModel(new ConfiguracaoTextBoxViewModel(dialogService, AlterouTabela), DataPreview, new EstadoDataFrameViewModel(_arquivo));

            _scriptManager = new(dialogService, conversor, contextModelo, contextArquivo, provider, mapper, CardsFuncoes, OpcoesPosicao, TextBox);

            Stopwatch = new Stopwatch();
        }

        [RelayCommand]
        private void ExportarCsvResultado()
        {
            if(_resultadosDataFrame.QuantidadeLinhas != 0)
            {
                var caminhoArquivo = _csvController.EscreveArquivo(_resultadosDataFrame);
                LancarNotificao(caminhoArquivo);
            }

        }

        [RelayCommand]
        private void ExportarCsvErros()
        {
            if(_listaErros!.Count != 0)
            {
                var caminhoArquivo = _csvController.EscreveArquivo(_listaErros);
                LancarNotificao(caminhoArquivo);
            }
        }

        private void LancarNotificao(string caminhoArquivo)
        {
            _notificationService.Notify(
                "Arquivo salvo em Downloads",
                "Abrir",
                () =>
                {
                    Process.Start("explorer.exe", Path.GetDirectoryName(caminhoArquivo)!);
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

            var nomeModelo = _contextModelo.RecebeMensagem().NomeModelo;
            var caminhoModelo = _contextModelo.RecebeMensagem().CaminhoPasta;

            var modelo = await _conversor.CarregarJsonAsync<ModeloEmUsoConfiguracao>(nomeModelo);

            var _schema = (await _conversor.CarregarJsonAsync<List<SchemaConfiguracao>>(nomeModelo)).First(p => p.Versao == modelo.SchemaVersao);

            PipelineTratamentoConfiguracao? pipeline = null;
            if (!string.IsNullOrWhiteSpace(modelo.PipelineVersao))
            {
                pipeline = (await _conversor.CarregarJsonAsync<List<PipelineTratamentoConfiguracao>>(nomeModelo)).First(p => p.Versao == modelo.PipelineVersao);
            }

            TransformadorConfiguracao? transformadores = null;
            if (!string.IsNullOrWhiteSpace(modelo.TransformadoresVersao))
            {
                transformadores = (await _conversor.CarregarJsonAsync<List<TransformadorConfiguracao>>(nomeModelo)).First(p => p.Versao == modelo.TransformadoresVersao);
            }

            var resultados = await _inferencia.RealizaInferenciaAsync(
                await CarregarDataFrameAsync(),
                _schema,
                pipeline,
                transformadores,
                caminhoModelo
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
                _resultadosDataFrame = await _scriptManager.CarregarPipeline(nomeModelo);
            }
            else
            {
                _resultadosDataFrame = TextBox.CarregarDados();
                TextBox.AtualizaTabela(_resultadosDataFrame);
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