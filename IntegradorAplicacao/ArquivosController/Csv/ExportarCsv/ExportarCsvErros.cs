using IntegradorDominio.Inferencia;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IntegradorAplicacao.ArquivosController.Csv.ExportarCsv
{
    public class ExportarCsvErros : ICsvExportador<List<ErrosInferencia>>
    {
        public string ExportarCsv(List<ErrosInferencia> erros)
        {
            var caminhoArquivo = ObterCaminhoDownloads(GerarNomeArquivo());
            SalvarResultadoEmDownloads(erros, caminhoArquivo);
            return caminhoArquivo;
        }

        private void SalvarResultadoEmDownloads(List<ErrosInferencia> erros, string caminhoArquivo)
        {
            if (erros == null || erros.Count == 0)
                return;

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

            header.Add("ID");

            foreach (var kv in outputMap)
                header.Add(kv.Key);

            header.Add("Erro");

            linhas.Add(string.Join(",", header));

            foreach (var erro in erros)
            {
                var linha = new List<string>();

                linha.Add(erro.Id);

                foreach (var kv in outputMap)
                {
                    var nome = kv.Key;
                    var tamanho = kv.Value.Count;

                    if (erro.Outputs.TryGetValue(nome, out var valor))
                    {
                        linha.Add(valor is IFormattable f ? f.ToString(null, CultureInfo.InvariantCulture) : valor?.ToString() ?? ""); 
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

            File.WriteAllLines(caminhoArquivo, linhas, Encoding.UTF8);
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
