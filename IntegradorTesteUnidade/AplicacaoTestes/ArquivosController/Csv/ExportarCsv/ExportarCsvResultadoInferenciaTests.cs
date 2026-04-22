using IntegradorAplicacao.ArquivosController.Csv.ExportarCsv;
using IntegradorDominio.Inferencia;

namespace IntegradorTesteUnidade.AplicacaoTestes.ArquivosController.Csv.ExportarCsv
{
    public class ExportarCsvResultadoInferenciaTests : IDisposable
    {
        private readonly List<string> _arquivosGerados = new();

        public void Dispose()
        {
            // limpa arquivos gerados no Downloads
            var pastaUsuario = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var downloads = Path.Combine(pastaUsuario, "Downloads");

            foreach (var file in Directory.GetFiles(downloads, "resultado_*.csv"))
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    // ignora erro de cleanup
                }
            }
        }

        private ExportarCsvResultadoInferencia CriarExportador()
        {
            return new ExportarCsvResultadoInferencia();
        }

        [Fact]
        public void ExportarCsv_DeveFazerNada_QuandoListaVazia()
        {
            var exportador = CriarExportador();

            var ex = Record.Exception(() => exportador.ExportarCsv(new List<ResultadoInferencia>()));

            Assert.Null(ex);
        }

        [Fact]
        public void ExportarCsv_DeveCriarArquivoComSucesso()
        {
            // Arrange
            var exportador = CriarExportador();

            var resultados = new List<ResultadoInferencia>
            {
                new ResultadoInferencia
                {
                    Id = "1",
                    Outputs = new Dictionary<string, float[]>
                    {
                        { "score", new float[] { 0.1f, 0.9f } }
                    }
                }
            };

            var pastaUsuario = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var downloads = Path.Combine(pastaUsuario, "Downloads");

            var arquivosAntes = Directory.GetFiles(downloads, "resultado_*.csv").Length;

            // Act
            exportador.ExportarCsv(resultados);

            // Assert
            var arquivosDepois = Directory.GetFiles(downloads, "resultado_*.csv");

            Assert.True(arquivosDepois.Length > arquivosAntes);

            var ultimoArquivo = arquivosDepois
                .OrderByDescending(f => File.GetCreationTime(f))
                .First();

            var conteudo = File.ReadAllText(ultimoArquivo);

            Assert.Contains("ID", conteudo);
            Assert.Contains("score_0", conteudo);
            Assert.Contains("0.1", conteudo);
            Assert.Contains("0.9", conteudo);
        }

        [Fact]
        public void ExportarCsv_DeveEscaparCsvCorretamente()
        {
            var exportador = CriarExportador();

            var resultados = new List<ResultadoInferencia>
            {
                new ResultadoInferencia
                {
                    Id = "id,com,virgula",
                    Outputs = new Dictionary<string, float[]>()
                }
            };

            exportador.ExportarCsv(resultados);

            var pastaUsuario = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var downloads = Path.Combine(pastaUsuario, "Downloads");

            var arquivo = Directory.GetFiles(downloads, "resultado_*.csv")
                .OrderByDescending(f => File.GetCreationTime(f))
                .First();

            var conteudo = File.ReadAllText(arquivo);

            Assert.Contains("\"id,com,virgula\"", conteudo);
        }
    }
}
