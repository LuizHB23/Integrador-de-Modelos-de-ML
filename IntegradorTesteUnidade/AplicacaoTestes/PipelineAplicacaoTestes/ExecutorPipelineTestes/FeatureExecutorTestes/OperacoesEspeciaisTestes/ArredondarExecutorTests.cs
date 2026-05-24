using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEspeciais;
using IntegradorDominio.FeatureEngineering.OperacoesEspeciais;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesEspeciaisTestes
{
    public class ArredondarExecutorTests
    {
        [Fact]
        public void DeveArredondarValoresCorretamente()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("Valores", new List<Single?> { 1.2345f, 2.7182f, -3.1415f, 0f, null });

            var operacao = new Arredondar { col = "Valores", value = "2" }; // arredonda para 2 casas
            var executor = new ArredondarExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);
            var col = resultado.PegarColuna<Single?>("Valores");

            Assert.Equal(1.23f, col.PegarValor(0));
            Assert.Equal(2.72f, col.PegarValor(1));
            Assert.Equal(-3.14f, col.PegarValor(2));
            Assert.Equal(0f, col.PegarValor(3));
            Assert.Null(col.PegarValor(4)); // mantém null
        }
    }
}
