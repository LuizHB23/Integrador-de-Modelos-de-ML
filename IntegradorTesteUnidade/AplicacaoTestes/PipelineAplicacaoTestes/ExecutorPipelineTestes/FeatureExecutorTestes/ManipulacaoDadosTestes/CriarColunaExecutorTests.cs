using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.ManipulacaoDados;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.ManipulacaoDadosTestes
{
    public class CriarColunaExecutorTests
    {
        [Fact]
        public void DeveCriarColunaComValorFixo_Single()
        {
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1, 2, 3 });

            var operacao = new CriarColuna { name = "Nova", type = "single", value = "10" };
            var executor = new CriarColunaExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<float?>("Nova");

            Assert.NotNull(col);
            Assert.Equal(3, col.Quantidade);
            Assert.All(col.Dados, v => Assert.Equal(10f, v));
        }

        [Fact]
        public void DeveCriarColunaComValorFixo_Boolean()
        {
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1, 2 });

            var operacao = new CriarColuna { name = "Flag", type = "bool", value = "true" };
            var executor = new CriarColunaExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<bool?>("Flag");
            Assert.NotNull(col);
            Assert.Equal(2, col.Quantidade);
            Assert.All(col.Dados, v => Assert.True(v.Value));
        }

        [Fact]
        public void DeveCriarColunaComValorFixo_String()
        {
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1, 2, 3 });

            var operacao = new CriarColuna { name = "Nome", type = "string", value = "Teste" };
            var executor = new CriarColunaExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<string>("Nome");
            Assert.NotNull(col);
            Assert.Equal(3, col.Quantidade);
            Assert.All(col.Dados, v => Assert.Equal("Teste", v));
        }

        [Fact]
        public void DeveCriarColunaAPartirDeOutroDataFrame()
        {
            // DataFrame base
            var dfBase = new DataFrame();
            dfBase.AdicionarColuna<float?>("X", new List<float?> { 1, 2, 3 });

            // DataFrame principal
            var df = new DataFrame();
            df.AdicionarColuna<float?>("Id", new List<float?> { 10, 20, 30 });

            var contexto = new Dictionary<string, object?> { { "dfBase", dfBase } };

            var operacao = new CriarColuna { name = "Copiada", value = "dfBase", Contexto = contexto };
            var executor = new CriarColunaExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<float?>("Copiada");
            Assert.NotNull(col);
            Assert.Equal(3, col.Quantidade);
            Assert.Equal(1, (float)col.PegarValor(0));
            Assert.Equal(2, (float)col.PegarValor(1));
            Assert.Equal(3, (float)col.PegarValor(2));
        }

        [Fact]
        public void DeveTratarValorVazioComoNulo()
        {
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1 });

            var operacao = new CriarColuna { name = "Nova", type = "single", value = "" };
            var executor = new CriarColunaExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<float?>("Nova");

            Assert.NotNull(col);
            Assert.Single(col.Dados);
            Assert.Null(col.PegarValor(0));
        }

        [Fact]
        public void DeveTratarStringVaziaComoStringVazia()
        {
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1 });

            var operacao = new CriarColuna { name = "Nova", type = "string", value = "" };
            var executor = new CriarColunaExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<string?>("Nova");

            Assert.NotNull(col);
            Assert.Single(col.Dados);
            Assert.NotNull(col.PegarValor(0));
        }
    }
}
