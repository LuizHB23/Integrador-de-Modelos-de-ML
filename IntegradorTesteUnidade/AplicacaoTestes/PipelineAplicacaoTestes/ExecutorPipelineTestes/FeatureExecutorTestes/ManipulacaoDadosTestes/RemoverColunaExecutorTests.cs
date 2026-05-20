using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.ManipulacaoDados;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.ManipulacaoDadosTestes
{
    public class RemoverColunaExecutorTests
    {
        [Fact]
        public void DeveRemoverUmaColuna()
        {
            var df = new DataFrame();
            df.AdicionarColuna<float?>("Id", new List<float?> { 1, 2 });
            df.AdicionarColuna<string?>("Nome", new List<string?> { "A", "B" });

            var operacao = new RemoverColuna { col = "[Id]" };
            var executor = new RemoverColunaExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            Assert.Null(resultado.PegarColuna<float?>("Id"));
            Assert.NotNull(resultado.PegarColuna<string>("Nome"));
            Assert.Equal(2, resultado.QuantidadeLinhas);
        }

        [Fact]
        public void DeveRemoverMultiplasColunas()
        {
            var df = new DataFrame();
            df.AdicionarColuna<float?>("Id", new List<float?> { 1 });
            df.AdicionarColuna<string?>("Nome", new List<string?> { "A" });
            df.AdicionarColuna<bool?>("Flag", new List<bool?> { true });

            var operacao = new RemoverColuna { col = "[Id, Flag]" };
            var executor = new RemoverColunaExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            Assert.Null(resultado.PegarColuna<float?>("Id"));
            Assert.Null(resultado.PegarColuna<bool?>("Flag"));
            Assert.NotNull(resultado.PegarColuna<string>("Nome"));
        }

        [Fact]
        public void NaoRemoveNadaSeColunaNaoExistir()
        {
            var df = new DataFrame();
            df.AdicionarColuna<float?>("Id", new List<float?> { 1 });
            df.AdicionarColuna<string?>("Nome", new List<string?> { "A" });

            var operacao = new RemoverColuna { col = "[NaoExiste]" };
            var executor = new RemoverColunaExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            Assert.NotNull(resultado.PegarColuna<float?>("Id"));
            Assert.NotNull(resultado.PegarColuna<string>("Nome"));
            Assert.Equal(1, resultado.QuantidadeLinhas);
        }
    }
}
