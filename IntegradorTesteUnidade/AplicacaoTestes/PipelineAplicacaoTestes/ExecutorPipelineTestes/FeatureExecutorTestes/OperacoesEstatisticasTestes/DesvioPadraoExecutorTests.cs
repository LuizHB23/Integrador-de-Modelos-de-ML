using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEstatisticas;
using IntegradorDominio.FeatureEngineering.OperacoesEstatisticas;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesEstatisticasTestes
{
    public class DesvioPadraoExecutorTests
    {
        [Fact]
        public void DeveCalcularDesvioPadraoIgnorandoNulos()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("Valores", new List<Single?> { 2f, 4f, null, 4f, 4f, 5f, 5f, 7f, 9f });

            var operacao = new DesvioPadrao { col = "Valores" };
            var executor = new DesvioPadraoExecutor(operacao);

            var resultado = (Single)executor.Executar(df);

            // Desvio padrão populacional manual: sqrt(32/8) ≈ 2
            Assert.True(Math.Abs(resultado - 2f) < 0.0001f);
        }

        [Fact]
        public void DeveRetornarZeroSeTodosValoresForemIguais()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("Valores", new List<Single?> { 3f, 3f, 3f, 3f });

            var operacao = new DesvioPadrao { col = "Valores" };
            var executor = new DesvioPadraoExecutor(operacao);

            var resultado = (Single)executor.Executar(df);

            Assert.Equal(0f, resultado);
        }

        [Fact]
        public void DeveRetornarZeroSeNaoExistiremValoresValidos()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("Valores", new List<Single?> { null, null });

            var operacao = new DesvioPadrao { col = "Valores" };
            var executor = new DesvioPadraoExecutor(operacao);

            var resultado = (Single)executor.Executar(df);

            Assert.Equal(0f, resultado);
        }
    }
}
