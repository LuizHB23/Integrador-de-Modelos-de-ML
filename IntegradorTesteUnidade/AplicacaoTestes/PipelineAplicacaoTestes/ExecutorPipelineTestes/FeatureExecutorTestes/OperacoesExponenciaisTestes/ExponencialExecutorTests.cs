using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesExponenciais;
using IntegradorDominio.FeatureEngineering.OperacoesExponenciais;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesExponenciaisTestes
{
    public class ExponencialExecutorTests
    {
        [Fact]
        public void Executar_DeveAplicarExponencialCorretamente()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { 0f, 1f, 2f });

            var operacao = new Exponencial { col = "Valores" };
            var executor = new ExponencialExecutor(operacao);

            // Act
            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            // Assert
            Assert.True(Math.Abs((Single)resultado.PegarValor(0)! - 1f) < 0.0001f); // e^0 = 1
            Assert.True(Math.Abs((Single)resultado.PegarValor(1)! - (Single)Math.Exp(1)) < 0.0001f); // e^1
            Assert.True(Math.Abs((Single)resultado.PegarValor(2)! - (Single)Math.Exp(2)) < 0.0001f); // e^2
        }

        [Fact]
        public void Executar_ComValoresNulos_DevePreservarNulos()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { null, 1f, null });

            var operacao = new Exponencial { col = "Valores" };
            var executor = new ExponencialExecutor(operacao);

            // Act
            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            // Assert
            Assert.Null(resultado.PegarValor(0));
            Assert.True(Math.Abs((Single)resultado.PegarValor(1)! - (Single)Math.Exp(1)) < 0.0001f);
            Assert.Null(resultado.PegarValor(2));
        }

        [Fact]
        public void Executar_ColunaVazia_DeveManterVazio()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?>());

            var operacao = new Exponencial { col = "Valores" };
            var executor = new ExponencialExecutor(operacao);

            // Act
            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            // Assert
            Assert.Equal(0, resultado.Quantidade);
        }

        [Fact]
        public void Executar_ValoresNegativos_DeveCalcularCorretamente()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { -1f, -2f });

            var operacao = new Exponencial { col = "Valores" };
            var executor = new ExponencialExecutor(operacao);

            // Act
            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            // Assert
            Assert.True(Math.Abs((Single)resultado.PegarValor(0)! - (Single)Math.Exp(-1)) < 0.0001f);
            Assert.True(Math.Abs((Single)resultado.PegarValor(1)! - (Single)Math.Exp(-2)) < 0.0001f);
        }
    }
}
