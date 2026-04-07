using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesAritmeticas;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;
using IntegradorDominio.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesAritmeticasTestes
{
    public class DividirExecutorTests
    {
        [Fact]
        public void DeveDividirDuasColunas()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("A", new List<Single?> { 10f, 20f, 30f });
            df.AdicionarColuna<Single?>("B", new List<Single?> { 2f, 4f, 5f });

            var operacao = new Dividir { left = "A", right = "B", exit = "Resultado" };
            var executor = new DividirExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<Single?>("Resultado");

            Assert.Equal(5f, col.PegarValor(0));
            Assert.Equal(5f, col.PegarValor(1));
            Assert.Equal(6f, col.PegarValor(2));
        }

        [Fact]
        public void DeveDividirColunaPorValor()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("A", new List<Single?> { 10f, 20f, 30f });

            var operacao = new Dividir { left = "A", right = null, value = "2", exit = "Resultado" };
            var executor = new DividirExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<Single?>("Resultado");

            Assert.Equal(5f, col.PegarValor(0));
            Assert.Equal(10f, col.PegarValor(1));
            Assert.Equal(15f, col.PegarValor(2));
        }

        [Fact]
        public void DeveDividirValorPorColuna()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("B", new List<Single?> { 2f, 4f, 5f });

            var operacao = new Dividir { left = null, right = "B", value = "10", exit = "Resultado" };
            var executor = new DividirExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<Single?>("Resultado");

            Assert.Equal(5f, col.PegarValor(0));
            Assert.Equal(2.5f, col.PegarValor(1));
            Assert.Equal(2f, col.PegarValor(2));
        }
    }
}
