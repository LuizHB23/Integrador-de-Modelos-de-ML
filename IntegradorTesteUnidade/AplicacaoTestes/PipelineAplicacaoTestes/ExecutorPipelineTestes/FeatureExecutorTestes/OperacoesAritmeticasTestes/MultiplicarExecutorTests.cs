using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesAritmeticas;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesAritmeticasTestes
{
    public class MultiplicarExecutorTests
    {
        [Fact]
        public void DeveMultiplicarDuasColunasNumericas()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("A", new List<Single?> { 2f, 3f, 4f });
            df.AdicionarColuna<Single?>("B", new List<Single?> { 5f, 6f, 7f });

            var operacao = new Multiplicar { left = "A", right = "B", exit = "Resultado" };
            var executor = new MultiplicarExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<Single?>("Resultado");

            Assert.Equal(10f, col.PegarValor(0));
            Assert.Equal(18f, col.PegarValor(1));
            Assert.Equal(28f, col.PegarValor(2));
        }

        [Fact]
        public void DeveMultiplicarColunaComValor()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("A", new List<Single?> { 2f, 3f, 4f });

            var operacao = new Multiplicar { left = "A", right = null, value = "3", exit = "Resultado" };
            var executor = new MultiplicarExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<Single?>("Resultado");

            Assert.Equal(6f, col.PegarValor(0));
            Assert.Equal(9f, col.PegarValor(1));
            Assert.Equal(12f, col.PegarValor(2));
        }

        [Fact]
        public void DeveMultiplicarValorComColuna()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("B", new List<Single?> { 5f, 6f, 7f });

            var operacao = new Multiplicar { left = null, right = "B", value = "2", exit = "Resultado" };
            var executor = new MultiplicarExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<Single?>("Resultado");

            Assert.Equal(10f, col.PegarValor(0));
            Assert.Equal(12f, col.PegarValor(1));
            Assert.Equal(14f, col.PegarValor(2));
        }
    }
}
