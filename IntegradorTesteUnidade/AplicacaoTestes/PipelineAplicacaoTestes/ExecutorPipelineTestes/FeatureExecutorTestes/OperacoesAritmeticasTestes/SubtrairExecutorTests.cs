using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesAritmeticas;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;
using IntegradorDominio.Models.DataFrameModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesAritmeticasTestes
{
    public class SubtrairExecutorTests
    {
        [Fact]
        public void DeveSubtrairDuasColunasNumericas()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("A", new List<Single?> { 10f, 20f, 30f });
            df.AdicionarColuna<Single?>("B", new List<Single?> { 1f, 2f, 3f });

            var operacao = new Subtrair { left = "A", right = "B", exit = "Resultado" };
            var executor = new SubtrairExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<Single?>("Resultado");

            Assert.Equal(9f, col.PegarValor(0));
            Assert.Equal(18f, col.PegarValor(1));
            Assert.Equal(27f, col.PegarValor(2));
        }

        [Fact]
        public void DeveSubtrairColunaComValor()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("A", new List<Single?> { 10f, 20f, 30f });

            var operacao = new Subtrair { left = "A", right = null, value = "5", exit = "Resultado" };
            var executor = new SubtrairExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<Single?>("Resultado");

            Assert.Equal(5f, col.PegarValor(0));
            Assert.Equal(15f, col.PegarValor(1));
            Assert.Equal(25f, col.PegarValor(2));
        }

        [Fact]
        public void DeveSubtrairValorComColuna()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("B", new List<Single?> { 1f, 2f, 3f });

            var operacao = new Subtrair { left = null, right = "B", value = "10", exit = "Resultado" };
            var executor = new SubtrairExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<Single?>("Resultado");

            Assert.Equal(9f, col.PegarValor(0));
            Assert.Equal(8f, col.PegarValor(1));
            Assert.Equal(7f, col.PegarValor(2));
        }

        [Fact]
        public void DeveSubtrairDuasColunasDateTime()
        {
            var df = new DataFrame();
            df.AdicionarColuna<DateTime?>("Inicio", new List<DateTime?> {
                new DateTime(2026, 1, 10),
                new DateTime(2026, 1, 15)
            });
            df.AdicionarColuna<DateTime?>("Fim", new List<DateTime?> {
                new DateTime(2026, 1, 5),
                new DateTime(2026, 1, 10)
            });

            var operacao = new Subtrair { left = "Inicio", right = "Fim", exit = "Dias" };
            var executor = new SubtrairExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<Single?>("Dias");

            Assert.Equal(5f, col.PegarValor(0));
            Assert.Equal(5f, col.PegarValor(1));
        }
    }
}
