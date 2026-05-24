using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEstatisticas;
using IntegradorDominio.FeatureEngineering.OperacoesEstatisticas;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesEstatisticasTestes
{
    public class ModaExecutorTests
    {
        [Fact]
        public void Executar_DeveRetornarModaSimples()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?>() { 1f, 2f, 2f, 3f, 3f, 3f });

            var operacao = new Moda { col = "Valores" };
            var executor = new ModaExecutor(operacao);

            // Act
            var resultado = executor.Executar(df);

            // Assert
            Assert.Equal(3f, resultado);
        }

        [Fact]
        public void Executar_DeveIgnorarValoresNulos()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?>() { 1f, null, 2f, 2f, null });

            var operacao = new Moda { col = "Valores" };
            var executor = new ModaExecutor(operacao);

            // Act
            var resultado = executor.Executar(df);

            // Assert
            Assert.Equal(2f, resultado);
        }

        [Fact]
        public void Executar_ColunaVazia_DeveRetornarNull()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?>());

            var operacao = new Moda { col = "Valores" };
            var executor = new ModaExecutor(operacao);

            // Act
            var resultado = executor.Executar(df);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public void Executar_MultiplasModas_DeveRetornarPrimeiraModa()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?>() { 1f, 1f, 2f, 2f });

            var operacao = new Moda { col = "Valores" };
            var executor = new ModaExecutor(operacao);

            // Act
            var resultado = executor.Executar(df);

            // Assert
            Assert.Equal(1f, resultado); // primeira moda encontrada
        }

        [Fact]
        public void Executar_TodosNulos_DeveRetornarNull()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?>() { null, null, null });

            var operacao = new Moda { col = "Valores" };
            var executor = new ModaExecutor(operacao);

            // Act
            var resultado = executor.Executar(df);

            // Assert
            Assert.Null(resultado);
        }
    }
}
