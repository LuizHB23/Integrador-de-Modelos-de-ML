using IntegradorAplicacao.ArquivosController.Csv.ExportarCsv;
using IntegradorDominio.Inferencia;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.ArquivosController.Csv.ExportarCsv
{
    public class ExportarCsvResultadoInferenciaTests : IDisposable
    {
        private readonly List<string> _arquivosGerados = new();

        [Fact]
        public void DeveGerarHeaderSimples()
        {
            var exporter = new ExportarCsvResultadoInferencia();

            var df = CriarDataFrameSimples();

            var caminho = exporter.ExportarCsv(df);
            _arquivosGerados.Add(caminho);

            var linhas = File.ReadAllLines(caminho);

            Assert.Equal("A,B", linhas[0]);
        }

        [Fact]
        public void DeveExpandirArraysNoHeader()
        {
            var exporter = new ExportarCsvResultadoInferencia();

            var df = CriarDataFrameArray();

            var caminho = exporter.ExportarCsv(df);
            _arquivosGerados.Add(caminho);

            var linhas = File.ReadAllLines(caminho);

            Assert.Contains("Valores_0", linhas[0]);
            Assert.Contains("Valores_1", linhas[0]);
        }

        [Fact]
        public void DevePreencherArrayComValoresFaltantes()
        {
            var exporter = new ExportarCsvResultadoInferencia();

            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<int[]?>
            {
                new int[] { 1, 2 },
                new int[] { 3 }
            });

            var caminho = exporter.ExportarCsv(df);
            _arquivosGerados.Add(caminho);

            var linhas = File.ReadAllLines(caminho);

            // linha 2 deve ter valor vazio no segundo slot
            Assert.Equal("3,", linhas[2]);
        }

        [Fact]
        public void DeveEscaparTextoComVirgula()
        {
            var exporter = new ExportarCsvResultadoInferencia();

            var df = new DataFrame();
            df.AdicionarColuna("Texto", new List<string?>
            {
                "a,b"
            });

            var caminho = exporter.ExportarCsv(df);
            _arquivosGerados.Add(caminho);

            var linhas = File.ReadAllLines(caminho);

            Assert.Equal("\"a,b\"", linhas[1]);
        }

        [Fact]
        public void DeveEscaparAspas()
        {
            var exporter = new ExportarCsvResultadoInferencia();

            var df = new DataFrame();
            df.AdicionarColuna("Texto", new List<string?>
            {
                "a\"b"
            });

            var caminho = exporter.ExportarCsv(df);
            _arquivosGerados.Add(caminho);

            var linhas = File.ReadAllLines(caminho);

            Assert.Equal("\"a\"\"b\"", linhas[1]);
        }

        [Fact]
        public void DataFrameVazio_NaoDeveGerarConteudo()
        {
            var exporter = new ExportarCsvResultadoInferencia();

            var df = new DataFrame();

            var caminho = exporter.ExportarCsv(df);

            // arquivo pode nem existir ou estar vazio
            if (File.Exists(caminho))
            {
                var linhas = File.ReadAllLines(caminho);
                Assert.Empty(linhas);
            }
        }

        // =========================
        // HELPERS
        // =========================

        private DataFrame CriarDataFrameSimples()
        {
            var df = new DataFrame();
            df.AdicionarColuna("A", new List<int?> { 1 });
            df.AdicionarColuna("B", new List<int?> { 2 });
            return df;
        }

        private DataFrame CriarDataFrameArray()
        {
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<int[]?>
            {
                new int[] { 1, 2 }
            });
            return df;
        }

        public void Dispose()
        {
            foreach (var arquivo in _arquivosGerados)
            {
                try
                {
                    if (File.Exists(arquivo))
                        File.Delete(arquivo);
                }
                catch { }
            }
        }
    }
}
