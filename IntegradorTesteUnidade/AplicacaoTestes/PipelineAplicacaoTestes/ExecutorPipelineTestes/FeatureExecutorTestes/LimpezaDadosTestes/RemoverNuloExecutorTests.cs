using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.LimpezaDados;
using IntegradorDominio.FeatureEngineering.LimpezaDados;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.LimpezaDadosTestes
{
    public class RemoverNuloExecutorTests
    {
        [Fact]
        public void DeveRemoverLinhasComNulos_Simples()
        {
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1, null, 2, 3 });
            df.AdicionarColuna<string?>("Nome", new List<string?> { "Ana", "Beto", null, "Cleo" });

            var executor = new RemoverNuloExecutor(new RemoverNulo());

            var resultado = (DataFrame)executor.Executar(df);

            Assert.Equal(2, resultado.QuantidadeLinhas);
            Assert.Equal(1, resultado.PegarColunaBase("Id")!.PegarValor(0));
            Assert.Equal(3, resultado.PegarColunaBase("Id")!.PegarValor(1));

            Assert.Equal("Ana", resultado.PegarColunaBase("Nome")!.PegarValor(0));
            Assert.Equal("Cleo", resultado.PegarColunaBase("Nome")!.PegarValor(1));
        }

        [Fact]
        public void DeveRemoverLinhasComStringsVazias()
        {
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1, 2, 3 });
            df.AdicionarColuna<string?>("Nome", new List<string?> { "Ana", "", "Cleo" });

            var executor = new RemoverNuloExecutor(new RemoverNulo());

            var resultado = (DataFrame)executor.Executar(df);

            Assert.Equal(2, resultado.QuantidadeLinhas);
            Assert.Equal(1, resultado.PegarColunaBase("Id")!.PegarValor(0));
            Assert.Equal(3, resultado.PegarColunaBase("Id")!.PegarValor(1));

            Assert.Equal("Ana", resultado.PegarColunaBase("Nome")!.PegarValor(0));
            Assert.Equal("Cleo", resultado.PegarColunaBase("Nome")!.PegarValor(1));
        }

        [Fact]
        public void DeveManterTudoSeNaoHouverNulos()
        {
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1, 2, 3 });
            df.AdicionarColuna<string?>("Nome", new List<string?> { "Ana", "Beto", "Cleo" });

            var executor = new RemoverNuloExecutor(new RemoverNulo());

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
        public void DeveRemoverTodasSeTudoNuloOuVazio()
        {
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { null, null });
            df.AdicionarColuna<string?>("Nome", new List<string?> { "", "" });

            var executor = new RemoverNuloExecutor(new RemoverNulo());

            var resultado = (DataFrame)executor.Executar(df);

            Assert.Equal(0, resultado.QuantidadeLinhas);
        }
    }
}
