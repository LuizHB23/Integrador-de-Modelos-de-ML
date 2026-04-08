using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEspeciais;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesEspeciais;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesEspeciaisTestes
{
    public class CeilExecutorTests
    {
        [Fact]
        public void DeveAplicarCeilNosValores()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("Valores", new List<Single?> { 1.2f, 2.8f, -3.4f, 0f, null });

            var operacao = new Ceil { col = "Valores" };
            var executor = new CeilExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);
            var col = resultado.PegarColuna<Single?>("Valores");

            Assert.Equal(2f, col.PegarValor(0));    // ceil de 1.2 -> 2
            Assert.Equal(3f, col.PegarValor(1));    // ceil de 2.8 -> 3
            Assert.Equal(-3f, col.PegarValor(2));   // ceil de -3.4 -> -3
            Assert.Equal(0f, col.PegarValor(3));    // 0 permanece
            Assert.Null(col.PegarValor(4));         // null permanece
        }
    }
}
