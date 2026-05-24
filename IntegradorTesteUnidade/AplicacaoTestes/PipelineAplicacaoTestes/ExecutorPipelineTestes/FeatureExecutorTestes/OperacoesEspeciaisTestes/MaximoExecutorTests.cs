using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEspeciais;
using IntegradorDominio.FeatureEngineering.OperacoesEspeciais;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesEspeciaisTestes
{
    public class MaximoExecutorTests
    {
        [Fact]
        public void DeveRetornarOMaiorValorIgnorandoNulos()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("Valores", new List<Single?> { 1.5f, 3.2f, null, 2.8f, 4.0f, -1.0f });

            var operacao = new Maximo { col = "Valores" };
            var executor = new MaximoExecutor(operacao);

            var resultado = executor.Executar(df);

            Assert.Equal(4.0f, resultado); // Maior valor
        }

        [Fact]
        public void DeveRetornarNullSeTodosValoresForemNulos()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("Valores", new List<Single?> { null, null });

            var operacao = new Maximo { col = "Valores" };
            var executor = new MaximoExecutor(operacao);

            var resultado = executor.Executar(df);

            Assert.Null(resultado); // Sem valores, retorna null
        }
    }
}
