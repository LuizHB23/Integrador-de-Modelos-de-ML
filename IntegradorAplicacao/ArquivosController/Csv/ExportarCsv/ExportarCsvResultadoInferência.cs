using IntegradorDominio.Inferencia;
using System.Globalization;
using System.Text;

namespace IntegradorAplicacao.ArquivosController.Csv.ExportarCsv
{
    public class ExportarCsvResultadoInferência : ICsvExportador<List<ResultadoInferencia>>
    {
        public void ExportarCsv(List<ResultadoInferencia> resultados)
        {
            SalvarResultadoEmDownloads(resultados);
        }

        private void SalvarResultadoEmDownloads(List<ResultadoInferencia> resultados)
        {
            if (resultados == null || resultados.Count == 0)
                throw new Exception("Nenhum resultado para exportar.");

            var caminho = ObterCaminhoDownloads(GerarNomeArquivo());

            var linhas = new List<string>();

            var outputMap = new Dictionary<string, int>();

            foreach (var r in resultados)
            {
                foreach (var kv in r.Outputs)
                {
                    if (kv.Value is float[] arr)
                    {
                        if (!outputMap.ContainsKey(kv.Key))
                            outputMap[kv.Key] = arr.Length;
                        else
                            outputMap[kv.Key] = Math.Max(outputMap[kv.Key], arr.Length);
                    }
                    else
                    {
                        outputMap[kv.Key] = 1;
                    }
                }
            }

            var header = new List<string> { "ID" };

            foreach (var kv in outputMap)
            {
                var nome = kv.Key;
                var tamanho = kv.Value;

                if (tamanho == 1)
                {
                    header.Add(nome);
                }
                else
                {
                    for (int i = 0; i < tamanho; i++)
                    {
                        header.Add($"{nome}_{i}");
                    }
                }
            }

            linhas.Add(string.Join(",", header));

            foreach (var r in resultados)
            {
                var linha = new List<string>
                {
                    EscaparCsv(r.Id) // garante segurança
                };

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
                                {
                                    linha.Add(arr[i].ToString(CultureInfo.InvariantCulture));
                                }
                                else
                                {
                                    linha.Add("");
                                }
                            }
                        }
                        else
                        {
                            linha.Add(valor.ToString());
                        }
                    }
                    else
                    {
                        for (int i = 0; i < tamanho; i++)
                        {
                            linha.Add("");
                        }
                    }
                }

                linhas.Add(string.Join(",", linha));
            }

            File.WriteAllLines(caminho, linhas, Encoding.UTF8);

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
            return $"resultado_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        }

        private string EscaparCsv(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return "";

            if (valor.Contains(",") || valor.Contains("\"") || valor.Contains("\n"))
            {
                valor = valor.Replace("\"", "\"\"");
                return $"\"{valor}\"";
            }

            return valor;
        }
    }
}
