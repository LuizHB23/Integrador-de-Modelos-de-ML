using IntegradorDominio.DataFrameModel;
using IntegradorDominio.Inferencia;
using System.Globalization;
using System.Text;

namespace IntegradorAplicacao.ArquivosController.Csv.ExportarCsv
{
    public class ExportarCsvResultadoInferencia : ICsvExportador<DataFrame>
    {
        public void ExportarCsv(DataFrame resultados)
        {
            SalvarResultadoEmDownloads(resultados);
        }

        private void SalvarResultadoEmDownloads(DataFrame resultados)
        {
            if (resultados == null || resultados.QuantidadeLinhas == 0)
                return;

            var caminho = ObterCaminhoDownloads(GerarNomeArquivo());

            var linhas = new List<string>();

            var header = new List<string>();
            var colunasExpandidas = new Dictionary<string, int>();

            // 📌 HEADER
            foreach (var coluna in resultados.Colunas)
            {
                if (typeof(Array).IsAssignableFrom(coluna.TipoDado))
                {
                    int maxSize = 0;

                    for (int i = 0; i < resultados.QuantidadeLinhas; i++)
                    {
                        if (coluna.PegarValor(i) is Array arr)
                            maxSize = Math.Max(maxSize, arr.Length);
                    }

                    colunasExpandidas[coluna.Nome] = maxSize;

                    for (int i = 0; i < maxSize; i++)
                    {
                        header.Add($"{coluna.Nome}_{i}");
                    }
                }
                else
                {
                    colunasExpandidas[coluna.Nome] = 1;
                    header.Add(coluna.Nome);
                }
            }

            linhas.Add(string.Join(",", header));

            // 📌 LINHAS
            for (int i = 0; i < resultados.QuantidadeLinhas; i++)
            {
                var linha = new List<string>();

                foreach (var coluna in resultados.Colunas)
                {
                    var valor = coluna.PegarValor(i);
                    int tamanho = colunasExpandidas[coluna.Nome];

                    if (valor is Array arr)
                    {
                        for (int j = 0; j < tamanho; j++)
                        {
                            if (j < arr.Length)
                                linha.Add(EscaparCsv(arr.GetValue(j)));
                            else
                                linha.Add("");
                        }
                    }
                    else
                    {
                        linha.Add(EscaparCsv(valor));
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

        private string EscaparCsv(object? valor)
        {
            if (valor == null)
                return "";

            var texto = Convert.ToString(valor, CultureInfo.InvariantCulture) ?? "";

            if (texto.Contains(",") || texto.Contains("\"") || texto.Contains("\n"))
            {
                texto = texto.Replace("\"", "\"\"");
                texto = $"\"{texto}\"";
            }

            return texto;
        }
    }
}
