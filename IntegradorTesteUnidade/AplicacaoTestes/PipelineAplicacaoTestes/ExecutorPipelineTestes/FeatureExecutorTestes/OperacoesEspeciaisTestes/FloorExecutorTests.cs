using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEspeciais;
using IntegradorDominio.FeatureEngineering.OperacoesEspeciais;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesEspeciaisTestes
{
    public class FloorExecutorTests
    {
        [Fact]
        public void DeveAplicarFloorNosValores()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("Valores", new List<Single?> { 1.7f, 2.3f, -3.8f, 0f, null });

            var operacao = new Floor { col = "Valores" };
            var executor = new FloorExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);
            var col = resultado.PegarColuna<Single?>("Valores");

            Assert.Equal(1f, col.PegarValor(0));    // floor de 1.7 -> 1
            Assert.Equal(2f, col.PegarValor(1));    // floor de 2.3 -> 2
            Assert.Equal(-4f, col.PegarValor(2));   // floor de -3.8 -> -4
            Assert.Equal(0f, col.PegarValor(3));    // 0 permanece
            Assert.Null(col.PegarValor(4));         // null permanece
        }
    }
}
