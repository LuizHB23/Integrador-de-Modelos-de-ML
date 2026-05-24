using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesExponenciais;
using IntegradorDominio.FeatureEngineering.OperacoesExponenciais;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesExponenciaisTestes
{
    public class PotenciaExecutorTests
    {
        [Fact]
        public void Executar_DeveElevarValoresAPotenciaCorretamente()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { 2f, 3f, 4f });

            var operacao = new Potencia { col = "Valores", value = "3" }; // Eleva ao cubo
            var executor = new PotenciaExecutor(operacao);

            // Act
            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            // Assert
            Assert.True(Math.Abs((Single)resultado.PegarValor(0)! - 8f) < 0.0001f);   // 2^3 = 8
            Assert.True(Math.Abs((Single)resultado.PegarValor(1)! - 27f) < 0.0001f);  // 3^3 = 27
            Assert.True(Math.Abs((Single)resultado.PegarValor(2)! - 64f) < 0.0001f);  // 4^3 = 64
        }

        [Fact]
        public void Executar_ComValoresNulos_DevePreservarNulos()
        {
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { null, 3f, null });

            var operacao = new Potencia { col = "Valores", value = "2" }; // Eleva ao quadrado
            var executor = new PotenciaExecutor(operacao);

            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            Assert.Null(resultado.PegarValor(0));
            Assert.True(Math.Abs((Single)resultado.PegarValor(1)! - 9f) < 0.0001f);
            Assert.Null(resultado.PegarValor(2));
        }

        [Fact]
        public void Executar_ValoresNegativos_DeveCalcularCorretamente()
        {
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { -2f, -3f });

            var operacao = new Potencia { col = "Valores", value = "2" }; // Eleva ao quadrado
            var executor = new PotenciaExecutor(operacao);

            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            Assert.True(Math.Abs((Single)resultado.PegarValor(0)! - 4f) < 0.0001f); // (-2)^2 = 4
            Assert.True(Math.Abs((Single)resultado.PegarValor(1)! - 9f) < 0.0001f); // (-3)^2 = 9
        }

        [Fact]
        public void Executar_ColunaVazia_DeveManterVazio()
        {
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?>());

            var operacao = new Potencia { col = "Valores", value = "3" };
            var executor = new PotenciaExecutor(operacao);

            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            Assert.Empty(resultado.Dados);
        }

        [Fact]
        public void Executar_PotenciaZero_DeveRetornarUm()
        {
            var df = new DataFrame();
            df.AdicionarColuna("Valores", new List<Single?> { 5f, -3f, 0f });

            var operacao = new Potencia { col = "Valores", value = "0" }; // qualquer número ^0 = 1
            var executor = new PotenciaExecutor(operacao);

            executor.Executar(df);
            var resultado = df.PegarColuna<Single?>("Valores");

            Assert.True(Math.Abs((Single)resultado.PegarValor(0)! - 1f) < 0.0001f);
            Assert.True(Math.Abs((Single)resultado.PegarValor(1)! - 1f) < 0.0001f);
            Assert.True(Math.Abs((Single)resultado.PegarValor(2)! - 1f) < 0.0001f);
        }
    }
}
