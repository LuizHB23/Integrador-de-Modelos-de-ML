using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesExponenciais;
using IntegradorDominio.FeatureEngineering.OperacoesExponenciais;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesExponenciaisTestes
{
    public class RaizQuadradaExecutorTests
    {
        [Fact]
        public void Executar_DeveCalcularRaizQuadradaCorretamente()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { 4f, 9f, 16f });

            var operacao = new RaizQuadrada { col = "Valores" };
            var executor = new RaizQuadradaExecutor(operacao);

            // Act
            executor.Executar(df);
            var resultado = df.PegarColunaBase("Valores");

            // Assert
            Assert.True(Math.Abs((Single)resultado.PegarValor(0)! - 2f) < 0.0001f);
            Assert.True(Math.Abs((Single)resultado.PegarValor(1)! - 3f) < 0.0001f);
            Assert.True(Math.Abs((Single)resultado.PegarValor(2)! - 4f) < 0.0001f);
        }

        [Fact]
        public void Executar_ComValoresNulos_DevePreservarNulos()
        {
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { null, 9f, null });

            var operacao = new RaizQuadrada { col = "Valores" };
            var executor = new RaizQuadradaExecutor(operacao);

            executor.Executar(df);
            var resultado = df.PegarColunaBase("Valores");

            Assert.Null(resultado.PegarValor(0));
            Assert.True(Math.Abs((Single)resultado.PegarValor(1)! - 3f) < 0.0001f);
            Assert.Null(resultado.PegarValor(2));
        }

        [Fact]
        public void Executar_ComZero_DevePreservarZero()
        {
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { 0f, 4f });

            var operacao = new RaizQuadrada { col = "Valores" };
            var executor = new RaizQuadradaExecutor(operacao);

            executor.Executar(df);
            var resultado = df.PegarColunaBase("Valores");

            Assert.Equal(0f, resultado.PegarValor(0)); // zero não é alterado
            Assert.True(Math.Abs((Single)resultado.PegarValor(1)! - 2f) < 0.0001f);
        }

        [Fact]
        public void Executar_ValoresNegativos_DevePreservarNegativos()
        {
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { -4f, -9f });

            var operacao = new RaizQuadrada { col = "Valores" };
            var executor = new RaizQuadradaExecutor(operacao);

            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            Assert.Equal(-4f, resultado.PegarValor(0)); // negativos não são alterados
            Assert.Equal(-9f, resultado.PegarValor(1));
        }

        [Fact]
        public void Executar_ColunaVazia_DeveManterVazio()
        {
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?>());

            var operacao = new RaizQuadrada { col = "Valores" };
            var executor = new RaizQuadradaExecutor(operacao);

            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            Assert.Empty(resultado.Dados);
        }
    }
}
