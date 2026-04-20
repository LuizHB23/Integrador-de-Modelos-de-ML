using IntegradorDominio.Inferencia;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IntegradorAplicacao.ArquivosController.Csv.ExportarCsv
{
    public class ExportarCsvErros : ICsvExportador<List<ErrosInferencia>>
    {
        public void ExportarCsv(List<ErrosInferencia> erros)
        {
            SalvarResultadoEmDownloads(erros);

            var caminho = ObterCaminhoDownloads(GerarNomeArquivo());

            var linhas = new List<string>();

            var outputMap = new Dictionary<string, List<object?>>();

            foreach (var erro in erros)
            {
                foreach (var kv in erro.Outputs)
                {
                    if (!outputMap.ContainsKey(kv.Key))
                        outputMap[kv.Key] = new List<object?> { kv.Value };
                    else
                        outputMap[kv.Key].Add(kv.Value);
                }
            }

            var header = new List<string>();

            foreach (var kv in outputMap)
                header.Add(kv.Key);

            header.Add("Erro");

            linhas.Add(string.Join(",", header));

            foreach (var erro in erros)
            {
                var linha = new List<string>();

                foreach (var kv in outputMap)
                {
                    var nome = kv.Key;
                    var tamanho = kv.Value.Count;

                    if (erro.Outputs.TryGetValue(nome, out var valor))
                    {
                        linha.Add(valor?.ToString() ?? "");
                    }
                    else
                    {
                        for (int i = 0; i < tamanho; i++)
                        {
                            linha.Add("");
                        }
                    }
                }

                linha.Add(erro.Erro);

                linhas.Add(string.Join(",", linha));
            }

            File.WriteAllLines(caminho, linhas, Encoding.UTF8);
        }

        private void SalvarResultadoEmDownloads(List<ErrosInferencia> erros)
        {
            if (erros == null || erros.Count == 0)
                return;
        }

        private string ObterCaminhoDownloads(string nomeArquivo)
        {
            var pastaUsuario = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var downloads = Path.Combine(pastaUsuario, "Downloads");

            Directory.CreateDirectory(downloads);

            return Path.Combine(downloads, nomeArquivo);
        }

        private string GerarNomeArquivo()
        {
            return $"erros_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        }
    }
}
