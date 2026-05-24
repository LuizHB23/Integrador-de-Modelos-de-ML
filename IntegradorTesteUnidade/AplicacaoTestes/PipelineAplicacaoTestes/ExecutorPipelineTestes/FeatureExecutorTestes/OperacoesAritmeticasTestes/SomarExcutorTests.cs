using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesAritmeticas;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesAritmeticasTestes
{
    public class SomarExcutorTests
    {
        [Fact]
        public void DeveSomarDuasColunas()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("A", new List<Single?> { 1f, 2f, 3f });
            df.AdicionarColuna<Single?>("B", new List<Single?> { 10f, 20f, 30f });

            var operacao = new Somar { left = "A", right = "B", exit = "Soma" };
            var executor = new SomarExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<Single?>("Soma");

            Assert.Equal(11f, col.PegarValor(0));
            Assert.Equal(22f, col.PegarValor(1));
            Assert.Equal(33f, col.PegarValor(2));
        }

        [Fact]
        public void DeveSomarColunaComValor()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("A", new List<Single?> { 1f, 2f, 3f });

            var operacao = new Somar { left = "A", right = null, value = "5", exit = "Soma" };
            var executor = new SomarExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<Single?>("Soma");

            Assert.Equal(6f, col.PegarValor(0));
            Assert.Equal(7f, col.PegarValor(1));
            Assert.Equal(8f, col.PegarValor(2));
        }

        [Fact]
        public void DeveSomarValorComColuna()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("B", new List<Single?> { 10f, 20f, 30f });

            var operacao = new Somar { left = null, right = "B", value = "2", exit = "Soma" };
            var executor = new SomarExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<Single?>("Soma");

            Assert.Equal(12f, col.PegarValor(0));
            Assert.Equal(22f, col.PegarValor(1));
            Assert.Equal(32f, col.PegarValor(2));
        }
    }
}
