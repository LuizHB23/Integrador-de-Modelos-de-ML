using IntegradorAplicacao.ArquivosController.Csv;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.PipelineAplicacao.ParserPipeline;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.Inferencia;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IntegradorViewModel.ControleUsuario.ConfiguracaoMetodo.EstadoDataFrame
{
    public class EstadoDataFrameViewModel
    {
        private readonly ArquivoDadosDTO _arquivoDados;

        private CsvController _controllerCsv;
        private IReadOnlyList<string>[]? _estadoColuna;
        private string[]? _estadoCabecalho;

        public EstadoDataFrameViewModel(ArquivoDadosDTO arquivoDados)
        {
            _arquivoDados = arquivoDados;
            _controllerCsv = new CsvController(arquivoDados.Delimitador, arquivoDados.Decimal, arquivoDados.Codificacao);
        }

        public DataFrame CarregarDados()
        {
            var dataFrame = new DataFrame();

            for (int i = 0; i < _estadoCabecalho.Length; i++)
            {
                var dado = _estadoColuna[i];
                dataFrame.AdicionarColuna(_estadoCabecalho[i], dado.ToList());
            }

            return dataFrame;
        }

        public async Task GuardaEstadoArquivo()
        {
            await _controllerCsv.CarregarArquivoAsync(_arquivoDados.CaminhoArquivoDados);

            _estadoCabecalho = _controllerCsv.Cabecalho;

            _estadoColuna = new IReadOnlyList<string>[_estadoCabecalho.Length];

            for (int i = 0; i < _estadoCabecalho.Length; i++)
            {
                _estadoColuna[i] = _controllerCsv.Colunas[i].ToList().AsReadOnly();
            }
        }

        public async Task GuardaEstadoResultado(List<ResultadoInferencia> resultados)
        {
            if (resultados == null || resultados.Count == 0)
                return;

            // 1. Mapear outputs (igual ao DataTable)
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

            // 2. Criar cabeçalho
            var headers = new List<string>();
            headers.Add("ID");

            foreach (var kv in outputMap)
            {
                if (kv.Value == 1)
                {
                    headers.Add(kv.Key);
                }
                else
                {
                    for (int i = 0; i < kv.Value; i++)
                    {
                        headers.Add($"{kv.Key}_{i}");
                    }
                }
            }

            _estadoCabecalho = headers.ToArray();

            // 3. Criar estrutura de colunas
            var listaColunas = new List<List<string>>();
            for (int i = 0; i < _estadoCabecalho.Length; i++)
            {
                listaColunas.Add(new List<string>());
            }

            // 4. Preencher dados (linha a linha)
            foreach (var r in resultados)
            {
                int colIndex = 0;

                // ID
                listaColunas[colIndex++].Add(r.Id);

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
                                if (i < arr.Length)
                                    listaColunas[colIndex++].Add(arr[i].ToString(CultureInfo.InvariantCulture));
                                else
                                    listaColunas[colIndex++].Add(null);
                            }
                        }
                        else
                        {
                            listaColunas[colIndex++].Add(valor?.ToString());
                        }
                    }
                    else
                    {
                        // preencher vazio
                        for (int i = 0; i < tamanho; i++)
                        {
                            listaColunas[colIndex++].Add(null);
                        }
                    }
                }
            }

            // 5. Converter para readonly
            _estadoColuna = new IReadOnlyList<string>[listaColunas.Count];

            for (int i = 0; i < listaColunas.Count; i++)
            {
                _estadoColuna[i] = listaColunas[i].AsReadOnly();
            }
        }
    }
}
