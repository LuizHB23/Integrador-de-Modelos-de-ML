using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEstatisticas;
using IntegradorDominio.FeatureEngineering.OperacoesEstatisticas;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesEstatisticasTestes
{
    public class MedianaExecutorTests
    {
        [Fact]
        public void DeveCalcularMedianaIgnorandoNulos()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("Valores", new List<Single?> { 2f, 4f, null, 4f, 4f, 5f, 5f, 7f, 9f });

            var operacao = new Mediana { col = "Valores" };
            var executor = new MedianaExecutor(operacao);

            var resultado = (Single)executor.Executar(df);

            // Valores válidos: [2, 4, 4, 4, 5, 5, 7, 9]
            // Ordenados: [2, 4, 4, 4, 5, 5, 7, 9]
            // Mediana (n par = 8): (4 + 5)/2 = 4.5
            Assert.Equal(4.5f, resultado);
        }

        [Fact]
        public void DeveRetornarZeroQuandoTodosValoresSaoNulos()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("Valores", new List<Single?> { null, null, null });

            var operacao = new Mediana { col = "Valores" };
            var executor = new MedianaExecutor(operacao);

            var resultado = (Single)executor.Executar(df);

            Assert.Equal(0f, resultado);
        }

        [Fact]
        public void DeveCalcularMedianaQuandoQuantidadeImpar()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("Valores", new List<Single?> { 1f, 3f, null, 2f, 5f });

            var operacao = new Mediana { col = "Valores" };
            var executor = new MedianaExecutor(operacao);

            var resultado = (Single)executor.Executar(df);

            // Valores válidos: [1, 2, 3, 5]
            // Ordenados: [1, 2, 3, 5]
            // Mediana (n par = 4): (2 + 3)/2 = 2.5
            Assert.Equal(2.5f, resultado);
        }
    }
}
