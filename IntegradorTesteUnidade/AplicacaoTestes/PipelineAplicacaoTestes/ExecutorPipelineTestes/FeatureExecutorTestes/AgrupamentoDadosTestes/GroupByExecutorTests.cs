using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.AgrupamentoDados;
using IntegradorDominio.FeatureEngineering.AgrupamentoDados;
using IntegradorDominio.Models.DataFrameModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.AgrupamentoDadosTestes
{
    public class GroupByExecutorTests
    {
        private DataFrame CriarDataFrameExemplo()
        {
            var df = new DataFrame();

            // Coluna de chave
            df.AdicionarColuna("CustomerID", new List<string> { "Ana", "Ana", "Beto", "Beto", "Beto" });

            // Coluna de valores
            df.AdicionarColuna("Valor", new List<Single?> { 10f, 20f, 5f, 15f, 25f });

            return df;
        }

        [Theory]
        [InlineData("sum", new float[] { 30f, 45f })]
        [InlineData("count", new float[] { 2f, 3f })]
        [InlineData("mean", new float[] { 15f, 15f })]
        [InlineData("min", new float[] { 10f, 5f })]
        [InlineData("max", new float[] { 20f, 25f })]
        public void TesteAgregacoesSimples(string agg, float[] esperado)
        {
            // Arrange
            var df = CriarDataFrameExemplo();
            var operacao = new GroupBy { col = "[CustomerID]", agg = agg };
            var executor = new GroupByExecutor(operacao);

            // Act
            var resultado = executor.Executar(df) as DataFrame;

            // Assert
            var colunaResultado = resultado!.PegarColuna<Single?>("Valor");
            for (int i = 0; i < esperado.Length; i++)
            {
                Assert.Equal(esperado[i], colunaResultado!.Get(i));
            }
        }

        [Fact]
        public void TesteDiffComoJanela()
        {
            // Arrange
            var df = CriarDataFrameExemplo();
            var operacao = new GroupBy { col = "[CustomerID]", agg = "diff" };
            var executor = new GroupByExecutor(operacao);

            // Act
            var resultado = executor.Executar(df) as DataFrame;

            // Assert
            var colunaResultado = resultado!.PegarColuna<Single?>("Valor");

            var valoresEsperados = new float?[] { null, 10f, null, 10f, 10f };

            for (int i = 0; i < valoresEsperados.Length; i++)
            {
                Assert.Equal(valoresEsperados[i], colunaResultado!.Get(i));
            }
        }

        [Fact]
        public void TesteChaveVaziaGeraErro()
        {
            // Arrange
            var df = CriarDataFrameExemplo();
            var operacao = new GroupBy { col = "[]", agg = "sum" };
            var executor = new GroupByExecutor(operacao);

            // Act & Assert
            Assert.Throws<InvalidCastException>(() => executor.Executar(df));
        }
    }
}
