using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesAritmeticas;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.OperacoesAritmeticasTestes
{
    public class ModExecutorTests
    {
        [Fact]
        public void DeveCalcularModuloCorretamente()
        {
            var df = new DataFrame();
            df.AdicionarColuna<Single?>("A", new List<Single?> { 10f, 15f, 20f, 7f });

            var operacao = new Mod { col = "A", value = "3", exit = "Resultado" };
            var executor = new ModExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            var col = resultado.PegarColuna<Single?>("Resultado");

            Assert.Equal(1f, col.PegarValor(0)); // 10 % 3 = 1
            Assert.Equal(0f, col.PegarValor(1)); // 15 % 3 = 0
            Assert.Equal(2f, col.PegarValor(2)); // 20 % 3 = 2
            Assert.Equal(1f, col.PegarValor(3)); // 7 % 3 = 1
        }

    }
}
