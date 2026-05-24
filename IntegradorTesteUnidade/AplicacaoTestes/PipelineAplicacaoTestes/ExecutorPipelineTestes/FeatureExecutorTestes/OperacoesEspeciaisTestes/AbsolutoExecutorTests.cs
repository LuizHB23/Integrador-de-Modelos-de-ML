using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEspeciais;
using IntegradorDominio.FeatureEngineering.OperacoesEspeciais;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesEspeciaisTestes
{
    public class AbsolutoExecutorTests
    {
        [Fact]
        public void DeveCalcularValorAbsolutoCorretamente()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("Valores", new List<Single?> { -10f, 5f, -3.5f, 0f, null });

            var operacao = new Absoluto { col = "Valores" };
            var executor = new AbsolutoExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<Single?>("Valores");

            Assert.Equal(10f, col.PegarValor(0));   // -10 => 10
            Assert.Equal(5f, col.PegarValor(1));    // 5 => 5
            Assert.Equal(3.5f, col.PegarValor(2));  // -3.5 => 3.5
            Assert.Equal(0f, col.PegarValor(3));    // 0 => 0
            Assert.Null(col.PegarValor(4));         // null => null
        }
    }
}
