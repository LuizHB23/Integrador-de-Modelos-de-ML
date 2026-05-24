using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEstatisticas;
using IntegradorDominio.FeatureEngineering.OperacoesEstatisticas;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesEstatisticasTestes
{
    public class VarianciaExecutorTests
    {
        [Fact]
        public void Executar_DeveCalcularVarianciaCorretamente()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { 1f, 2f, 3f, 4f, 5f });

            var operacao = new Variancia { col = "Valores" };
            var executor = new VarianciaExecutor(operacao);

            // Act
            var resultado = executor.Executar(df);

            // Assert
            // Variância populacional: ((1-3)^2 + (2-3)^2 + (3-3)^2 + (4-3)^2 + (5-3)^2) / 5 = 2
            Assert.Equal(2f, resultado);
        }

        [Fact]
        public void Executar_ComValoresNulos_DeveIgnorarNulos()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { 1f, null, 2f, 2f, null });

            var operacao = new Variancia { col = "Valores" };
            var executor = new VarianciaExecutor(operacao);

            // Act
            var resultado = executor.Executar(df);

            // Assert
            // Valores válidos: 1,2,2 → média = 1.6666667
            // Variância = ((1-1.6667)^2 + (2-1.6667)^2 + (2-1.6667)^2) / 3 ≈ 0.2222
            Assert.True(Math.Abs((Single)resultado - 0.2222222f) < 0.0001f);
        }

        [Fact]
        public void Executar_ColunaVazia_DeveRetornarZero()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?>());

            var operacao = new Variancia { col = "Valores" };
            var executor = new VarianciaExecutor(operacao);

            // Act
            var resultado = executor.Executar(df);

            // Assert
            Assert.Equal(0f, resultado);
        }

        [Fact]
        public void Executar_SomenteUmValor_DeveRetornarZero()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { 42f });

            var operacao = new Variancia { col = "Valores" };
            var executor = new VarianciaExecutor(operacao);

            // Act
            var resultado = executor.Executar(df);

            // Assert
            Assert.Equal(0f, resultado);
        }

        [Fact]
        public void Executar_TodosNulos_DeveRetornarZero()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { null, null, null });

            var operacao = new Variancia { col = "Valores" };
            var executor = new VarianciaExecutor(operacao);

            // Act
            var resultado = executor.Executar(df);

            // Assert
            Assert.Equal(0f, resultado);
        }
    }
}
