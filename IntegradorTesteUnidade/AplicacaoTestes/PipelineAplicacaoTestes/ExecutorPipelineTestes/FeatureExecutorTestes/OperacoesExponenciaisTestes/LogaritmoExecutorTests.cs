using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesExponenciais;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesExponenciais;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesExponenciaisTestes
{
    public class LogaritmoExecutorTests
    {
        [Fact]
        public void Executar_DeveCalcularLogNaturalCorretamente()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { 1f, (Single)Math.E, (Single)(Math.E * Math.E) });

            var operacao = new Logaritmo { col = "Valores" };
            var executor = new LogaritmoExecutor(operacao);

            // Act
            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            // Assert
            Assert.True(Math.Abs((Single)resultado.PegarValor(0)! - 0f) < 0.0001f);          // log(1) = 0
            Assert.True(Math.Abs((Single)resultado.PegarValor(1)! - 1f) < 0.0001f);          // log(e) = 1
            Assert.True(Math.Abs((Single)resultado.PegarValor(2)! - 2f) < 0.0001f);          // log(e^2) = 2
        }

        [Fact]
        public void Executar_ComValoresNulos_DevePreservarNulos()
        {
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { null, (Single)Math.E, null });

            var operacao = new Logaritmo { col = "Valores" };
            var executor = new LogaritmoExecutor(operacao);

            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            Assert.Null(resultado.PegarValor(0));
            Assert.True(Math.Abs((Single)resultado.PegarValor(1)! - 1f) < 0.0001f);
            Assert.Null(resultado.PegarValor(2));
        }

        [Fact]
        public void Executar_ComZero_DevePreservarZero()
        {
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { 0f, 1f });

            var operacao = new Logaritmo { col = "Valores" };
            var executor = new LogaritmoExecutor(operacao);

            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            Assert.Equal(0f, resultado.PegarValor(0)); // zero não é alterado
            Assert.True(Math.Abs((Single)resultado.PegarValor(1)! - 0f) < 0.0001f); // log(1) = 0
        }

        [Fact]
        public void Executar_ValoresNegativos_DevePreservarNegativos()
        {
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { -1f, -10f });

            var operacao = new Logaritmo { col = "Valores" };
            var executor = new LogaritmoExecutor(operacao);

            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            Assert.Equal(-1f, resultado.PegarValor(0)); // negativos não são alterados
            Assert.Equal(-10f, resultado.PegarValor(1));
        }

        [Fact]
        public void Executar_ColunaVazia_DeveManterVazio()
        {
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?>());

            var operacao = new Logaritmo { col = "Valores" };
            var executor = new LogaritmoExecutor(operacao);

            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            Assert.Equal(0, resultado.Quantidade);
        }
    }
}
