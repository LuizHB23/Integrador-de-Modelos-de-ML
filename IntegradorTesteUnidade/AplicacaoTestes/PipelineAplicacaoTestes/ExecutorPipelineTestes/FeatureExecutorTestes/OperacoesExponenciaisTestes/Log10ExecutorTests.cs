using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesExponenciais;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesExponenciais;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesExponenciaisTestes
{
    public class Log10ExecutorTests
    {
        [Fact]
        public void Executar_DeveCalcularLog10Corretamente()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { 1f, 10f, 100f });

            var operacao = new Log10 { col = "Valores" };
            var executor = new Log10Executor(operacao);

            // Act
            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            // Assert
            Assert.True(Math.Abs((Single)resultado.PegarValor(0)! - 0f) < 0.0001f);  // log10(1) = 0
            Assert.True(Math.Abs((Single)resultado.PegarValor(1)! - 1f) < 0.0001f);  // log10(10) = 1
            Assert.True(Math.Abs((Single)resultado.PegarValor(2)! - 2f) < 0.0001f);  // log10(100) = 2
        }

        [Fact]
        public void Executar_ComValoresNulos_DevePreservarNulos()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { null, 10f, null });

            var operacao = new Log10 { col = "Valores" };
            var executor = new Log10Executor(operacao);

            // Act
            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            // Assert
            Assert.Null(resultado.PegarValor(0));
            Assert.True(Math.Abs((Single)resultado.PegarValor(1)! - 1f) < 0.0001f);
            Assert.Null(resultado.PegarValor(2));
        }

        [Fact]
        public void Executar_ComZero_DevePreservarZero()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { 0f, 1f });

            var operacao = new Log10 { col = "Valores" };
            var executor = new Log10Executor(operacao);

            // Act
            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            // Assert
            Assert.Equal(0f, resultado.PegarValor(0)); // zero não é alterado
            Assert.True(Math.Abs((Single)resultado.PegarValor(1)! - 0f) < 0.0001f); // log10(1) = 0
        }

        [Fact]
        public void Executar_ColunaVazia_DeveManterVazio()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?>());

            var operacao = new Log10 { col = "Valores" };
            var executor = new Log10Executor(operacao);

            // Act
            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            // Assert
            Assert.Equal(0, resultado.Quantidade);
        }

        [Fact]
        public void Executar_ValoresNegativos_DevePreservarNegativos()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { -1f, -10f });

            var operacao = new Log10 { col = "Valores" };
            var executor = new Log10Executor(operacao);

            // Act
            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            // Assert
            Assert.Equal(-1f, resultado.PegarValor(0)); // valores negativos não são alterados
            Assert.Equal(-10f, resultado.PegarValor(1));
        }
    }
}
