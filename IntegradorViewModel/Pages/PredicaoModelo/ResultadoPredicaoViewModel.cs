using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.ArquivosController.Csv;
using IntegradorAplicacao.ArquivosController.Csv.ExportarCsv;
using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.InferenciaAplicacao;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.Inferencia;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
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
        private DataView _dataPreview;

        [ObservableProperty]
        private INavigationService _navigation;

        private readonly IContext<ArquivoDadosDTO> _contextArquivo;
        private readonly IContext<ModeloDTO> _contextModelo;

        private List<ResultadoInferencia>? _resultados;
        private List<ErrosInferencia>? _listaErros;
        private Inferencia _inferencia;
        private CsvController _csvController;

        public Stopwatch Stopwatch { get; set; }

        private ArquivoDadosDTO _arquivo { get; set; }

        public ResultadoPredicaoViewModel(INavigationService navigation, IContext<ModeloDTO> contextModelo, IContext<ArquivoDadosDTO> contextArquivo, Inferencia inferencia)
        {
            Navigation = navigation;
            DataPreview = new();

            _contextArquivo = contextArquivo;
            _contextModelo = contextModelo;

            _arquivo = contextArquivo.RecebeMensagem();

            _inferencia = inferencia;
            _csvController = new CsvController();

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

            _listaErros = _inferencia.ListaErros;

            stopwatch.Stop();
            cts.Cancel();

            TempoProcessamento = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.ff");

            DataPreview = ResultadoParaDataTable(_resultados).DefaultView;
        }

        private DataTable ResultadoParaDataTable(List<ResultadoInferencia> resultados)
        {
            var tabela = new DataTable();

            if (resultados == null || resultados.Count == 0)
                return tabela;

            tabela.Columns.Add("ID", typeof(string));

            // Mapeia: nomeOutput → tamanho vetor
            var outputMap = new Dictionary<string, int>();

            foreach (var r in resultados)
            {
                foreach (var kv in r.Outputs)
                {
                    if (kv.Value is float[] arr)
                    {
                        outputMap[kv.Key] = Math.Max(
                            outputMap.ContainsKey(kv.Key) ? outputMap[kv.Key] : 0,
                            arr.Length
                        );
                    }
                    else
                    {
                        outputMap[kv.Key] = 1;
                    }
                }
            }

            foreach (var kv in outputMap)
            {
                var nome = kv.Key;
                var tamanho = kv.Value;

                if (tamanho == 1)
                {
                    tabela.Columns.Add(nome, typeof(float));
                }
                else
                {
                    for (int i = 0; i < tamanho; i++)
                    {
                        tabela.Columns.Add($"{nome}_{i}", typeof(float));
                    }
                }
            }

            foreach (var r in resultados)
            {
                var linha = tabela.NewRow();

                linha["ID"] = r.Id;

                foreach (var kv in outputMap)
                {
                    var nome = kv.Key;
                    var tamanho = kv.Value;

                    if (r.Outputs.TryGetValue(nome, out var valor))
                    {
                        if (valor is float[] arr)
                        {
                            for (int i = 0; i < tamanho; i++)
                            {
                                var colName = tamanho == 1 ? nome : $"{nome}_{i}";

                                if (i < arr.Length)
                                    linha[colName] = arr[i];
                                else
                                    linha[colName] = DBNull.Value;
                            }
                        }
                        else
                        {
                            linha[nome] = valor;
                        }
                    }
                    else
                    {
                        // Preenche vazio
                        if (tamanho == 1)
                        {
                            linha[nome] = DBNull.Value;
                        }
                        else
                        {
                            for (int i = 0; i < tamanho; i++)
                            {
                                linha[$"{nome}_{i}"] = DBNull.Value;
                            }
                        }
                    }
                }

                tabela.Rows.Add(linha);
            }

            return tabela;
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