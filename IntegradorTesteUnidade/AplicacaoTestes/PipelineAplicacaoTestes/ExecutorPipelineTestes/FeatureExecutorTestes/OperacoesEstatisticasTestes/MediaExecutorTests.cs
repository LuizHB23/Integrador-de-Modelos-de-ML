using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEstatisticas;
using IntegradorDominio.FeatureEngineering.OperacoesEstatisticas;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesEstatisticasTestes
{
    public class MediaExecutorTests
    {
        [Fact]
        public void DeveCalcularMediaIgnorandoNulos()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("Valores", new List<Single?> { 2f, 4f, null, 4f, 4f, 5f, 5f, 7f, 9f });

            var operacao = new Media { col = "Valores" };
            var executor = new MediaExecutor(operacao);

            var resultado = (Single)executor.Executar(df);

            // Soma dos valores válidos: 2+4+4+4+5+5+7+9 = 40
            // Número de valores válidos: 8
            // Média = 40 / 8 = 5
            Assert.Equal(5f, resultado);
        }
    }
}
