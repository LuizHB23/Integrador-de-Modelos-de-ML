using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.LimpezaDados;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.LimpezaDados;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.LimpezaDadosTestes
{
    public class FillNaExecutorTests
    {
        [Fact]
        public void FillNa_PreencheValoresNulos()
        {
            // Arrange: criar DataFrame com colunas de vários tipos
            var df = new DataFrame();

            df.AdicionarColuna<float?>("ValorFloat", new List<float?> { null, 10f, null });
            df.AdicionarColuna<bool?>("ValorBool", new List<bool?> { true, null, false });
            df.AdicionarColuna<DateTime?>("ValorDate", new List<DateTime?> { null, DateTime.Parse("2026-04-07"), null });
            df.AdicionarColuna<string?>("ValorString", new List<string?> { "A", null, null });

            // Contexto opcional
            var contexto = new Dictionary<string, object?>();

            var operacaoFloat = new FillNa
            {
                col = "ValorFloat",
                value = "99",
                Contexto = contexto
            };
            var executorFloat = new FillNaExecutor(operacaoFloat);

            var operacaoBool = new FillNa
            {
                col = "ValorBool",
                value = "true",
                Contexto = contexto
            };
            var executorBool = new FillNaExecutor(operacaoBool);

            var operacaoDate = new FillNa
            {
                col = "ValorDate",
                value = "2026-01-01",
                Contexto = contexto
            };
            var executorDate = new FillNaExecutor(operacaoDate);

            var operacaoString = new FillNa
            {
                col = "ValorString",
                value = "Preenchido",
                Contexto = contexto
            };
            var executorString = new FillNaExecutor(operacaoString);

            // Act
            var resultado = executorFloat.Executar(df);
            resultado = executorBool.Executar((DataFrame)resultado);
            resultado = executorDate.Executar((DataFrame)resultado);
            resultado = executorString.Executar((DataFrame)resultado);

            // Assert
            var colunaFloat = ((DataFrame)resultado).PegarColunaBase("ValorFloat") as Coluna<float?>;
            Assert.Equal(new float?[] { 99f, 10f, 99f }, colunaFloat!.Dados);

            var colunaBool = ((DataFrame)resultado).PegarColunaBase("ValorBool") as Coluna<bool?>;
            Assert.Equal(new bool?[] { true, true, false }, colunaBool!.Dados);

            var colunaDate = ((DataFrame)resultado).PegarColunaBase("ValorDate") as Coluna<DateTime?>;
            Assert.Equal(new DateTime?[] { DateTime.Parse("2026-01-01"), DateTime.Parse("2026-04-07"), DateTime.Parse("2026-01-01") }, colunaDate!.Dados);

            var colunaString = ((DataFrame)resultado).PegarColunaBase("ValorString") as Coluna<string?>;
            Assert.Equal(new string?[] { "A", "Preenchido", "Preenchido" }, colunaString!.Dados);
        }

        [Fact]
        public void LancaExcecaoParaColunaInexistente()
        {
            var df = new DataFrame();
            var operacao = new FillNa { col = "Inexistente", value = "1", Contexto = null };
            var executor = new FillNaExecutor(operacao);

            Assert.Throws<Exception>(() => executor.Executar(df));
        }
    }
}
