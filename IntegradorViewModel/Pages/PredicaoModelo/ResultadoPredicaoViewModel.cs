using CommunityToolkit.Mvvm.ComponentModel;
using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.InferenciaAplicacao;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.Inferencia;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using System.Data;
using System.Text;

namespace IntegradorViewModel.Pages.PredicaoModelo
{
    public partial class ResultadoPredicaoViewModel : ObservableObject
    {
        [ObservableProperty]
        private DataView _dataPreview;

        [ObservableProperty]
        private INavigationService _navigation;

        private readonly IContext<ArquivoDadosDTO> _contextArquivo;
        private readonly IContext<ModeloDTO> _contextModelo;

        private Inferencia _inferencia;

        private ArquivoDadosDTO _arquivo { get; set; }

        public ResultadoPredicaoViewModel(INavigationService navigation, IContext<ModeloDTO> contextModelo, IContext<ArquivoDadosDTO> contextArquivo, Inferencia inferencia)
        {
            Navigation = navigation;
            DataPreview = new();

            _contextArquivo = contextArquivo;
            _contextModelo = contextModelo;

            _arquivo = contextArquivo.RecebeMensagem();

            _inferencia = inferencia;

            var caminhoModelo = _contextModelo.RecebeMensagem().CaminhoPasta;
            var caminhoPasta = Path.GetDirectoryName(caminhoModelo);
            var caminhoSchema = Path.Combine(caminhoPasta, "schema.json");
            var caminhoPipeline = Path.Combine(caminhoPasta, "pipeline.json");
            var caminhoTransformadores = Path.Combine(caminhoPasta, "transformador.json");


            var resultados = _inferencia.RealizaInferencia(CarregarDataFrame(), caminhoModelo, caminhoSchema, caminhoPipeline, caminhoTransformadores);

            DataPreview = ResultadoParaDataTable(resultados).DefaultView;
        }

        private static DataTable ResultadoParaDataTable(List<ResultadoInferencia> resultados)
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

        private DataFrame CarregarDataFrame()
        {
            var linhas = File.ReadAllLines(_arquivo.CaminhoArquivoDados);
            var estadoCabecalho = ParseCsvLine(linhas[0]);

            var estadoColuna = estadoCabecalho.Select(_ => new List<string>()).ToArray();

            for (int i = 1; i < linhas.Length; i++)
            {
                var partes = ParseCsvLine(linhas[i]);

                for (int j = 0; j < estadoColuna.Length; j++)
                {
                    if (j < partes.Length)
                        estadoColuna[j].Add(partes[j]);
                    else
                        estadoColuna[j].Add(string.Empty);
                }
            }

            var dataFrame = new DataFrame();

            for (int i = 0; i < estadoCabecalho.Length; i++)
            {
                var dado = estadoColuna[i];
                dataFrame.AdicionarColuna(estadoCabecalho[i], dado);
            }

            return dataFrame;
        }

        // Função simples para parsear CSV com aspas
        private string[] ParseCsvLine(string linha)
        {
            var resultado = new List<string>();
            bool dentroAspas = false;
            var buffer = new StringBuilder();

            foreach (char c in linha)
            {
                if (c == '"')
                {
                    dentroAspas = !dentroAspas; // alterna estado
                    continue; // remove as aspas
                }

                if (c == ',' && !dentroAspas)
                {
                    resultado.Add(buffer.ToString());
                    buffer.Clear();
                }
                else
                {
                    buffer.Append(c);
                }
            }

            resultado.Add(buffer.ToString()); // adiciona último campo
            return resultado.ToArray();
        }
    }
}




