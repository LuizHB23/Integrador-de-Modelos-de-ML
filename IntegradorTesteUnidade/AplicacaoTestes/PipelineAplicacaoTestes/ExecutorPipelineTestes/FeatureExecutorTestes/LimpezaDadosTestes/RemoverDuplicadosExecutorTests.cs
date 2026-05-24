using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.LimpezaDados;
using IntegradorDominio.FeatureEngineering.LimpezaDados;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.LimpezaDadosTestes
{
    public class RemoverDuplicadosExecutorTests
    {
        [Fact]
        public void DeveRemoverLinhasDuplicadas_Simples()
        {
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1, 2, 2, 3, 1 });
            df.AdicionarColuna<string?>("Nome", new List<string?> { "Ana", "Beto", "Beto", "Cleo", "Ana" });

            var executor = new RemoverDuplicadosExecutor(new RemoverDuplicados());

            var resultado = (DataFrame)executor.Executar(df);

            Assert.Equal(3, resultado.QuantidadeLinhas);
            Assert.Equal(1, resultado.PegarColunaBase("Id")!.PegarValor(0));
            Assert.Equal(2, resultado.PegarColunaBase("Id")!.PegarValor(1));
            Assert.Equal(3, resultado.PegarColunaBase("Id")!.PegarValor(2));

            Assert.Equal("Ana", resultado.PegarColunaBase("Nome")!.PegarValor(0));
            Assert.Equal("Beto", resultado.PegarColunaBase("Nome")!.PegarValor(1));
            Assert.Equal("Cleo", resultado.PegarColunaBase("Nome")!.PegarValor(2));
        }

        [Fact]
        public void DeveRemoverDuplicados_ComNulls()
        {
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1, null, null, 2 });
            df.AdicionarColuna<string?>("Nome", new List<string?> { "Ana", null, null, "Beto" });

            var executor = new RemoverDuplicadosExecutor(new RemoverDuplicados());

            var resultado = (DataFrame)executor.Executar(df);

            Assert.Equal(3, resultado.QuantidadeLinhas);
            Assert.Equal(1, resultado.PegarColunaBase("Id")!.PegarValor(0));
            Assert.Null(resultado.PegarColunaBase("Id")!.PegarValor(1));
            Assert.Equal(2, resultado.PegarColunaBase("Id")!.PegarValor(2));

            Assert.Equal("Ana", resultado.PegarColunaBase("Nome")!.PegarValor(0));
            Assert.Null(resultado.PegarColunaBase("Nome")!.PegarValor(1));
            Assert.Equal("Beto", resultado.PegarColunaBase("Nome")!.PegarValor(2));
        }

        [Fact]
        public void DeveManterTudoSeNaoHouverDuplicatas()
        {
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1, 2, 3 });
            df.AdicionarColuna<string?>("Nome", new List<string?> { "Ana", "Beto", "Cleo" });

            var executor = new RemoverDuplicadosExecutor(new RemoverDuplicados());

            var resultado = (DataFrame)executor.Executar(df);

            Assert.Equal(3, resultado.QuantidadeLinhas);
            Assert.Equal(1, resultado.PegarColunaBase("Id")!.PegarValor(0));
            Assert.Equal(2, resultado.PegarColunaBase("Id")!.PegarValor(1));
            Assert.Equal(3, resultado.PegarColunaBase("Id")!.PegarValor(2));

            Assert.Equal("Ana", resultado.PegarColunaBase("Nome")!.PegarValor(0));
            Assert.Equal("Beto", resultado.PegarColunaBase("Nome")!.PegarValor(1));
            Assert.Equal("Cleo", resultado.PegarColunaBase("Nome")!.PegarValor(2));
        }

        [Fact]
        public void DeveReduzirParaUmaLinhaSeTudoDuplicado()
        {
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1, 1, 1 });
            df.AdicionarColuna<string?>("Nome", new List<string?> { "Ana", "Ana", "Ana" });

            var executor = new RemoverDuplicadosExecutor(new RemoverDuplicados());

            var resultado = (DataFrame)executor.Executar(df);

            Assert.Equal(1, resultado.QuantidadeLinhas);
            Assert.Equal(1, resultado.PegarColunaBase("Id")!.PegarValor(0));
            Assert.Equal("Ana", resultado.PegarColunaBase("Nome")!.PegarValor(0));
        }
    }
}
