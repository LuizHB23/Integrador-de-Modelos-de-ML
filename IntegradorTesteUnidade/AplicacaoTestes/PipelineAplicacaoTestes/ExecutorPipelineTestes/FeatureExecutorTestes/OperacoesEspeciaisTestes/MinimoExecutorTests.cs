using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEspeciais;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesEspeciais;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesEspeciaisTestes
{
    public class MinimoExecutorTests
    {
        [Fact]
        public void DeveRetornarOMenorValorIgnorandoNulos()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("Valores", new List<Single?> { 1.5f, 3.2f, null, 2.8f, -4.0f, 0.0f });

            var operacao = new Minimo { col = "Valores" };
            var executor = new MinimoExecutor(operacao);

            var resultado = executor.Executar(df);

            Assert.Equal(-4.0f, resultado); // Menor valor
        }

        [Fact]
        public void DeveRetornarNullSeTodosValoresForemNulos()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("Valores", new List<Single?> { null, null });

            var operacao = new Minimo { col = "Valores" };
            var executor = new MinimoExecutor(operacao);

            var resultado = executor.Executar(df);

            Assert.Null(resultado); // Sem valores, retorna null
        }
    }
}
