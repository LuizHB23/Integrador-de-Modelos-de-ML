using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.ManipulacaoDados;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using IntegradorDominio.Models.DataFrameModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.ManipulacaoDadosTestes
{
    public class SelecionarColunaExecutorTests
    {
        [Fact]
        public void DeveSelecionarUmaColuna()
        {
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1, 2, 3 });
            df.AdicionarColuna<string?>("Nome", new List<string?> { "A", "B", "C" });

            var operacao = new SelecionarColuna { col = "Id" };
            var executor = new SelecionarColunaExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            Assert.Single(resultado.Colunas);
            Assert.NotNull(resultado.PegarColuna<int?>("Id"));
            Assert.Null(resultado.PegarColuna<string?>("Nome"));
        }

        [Fact]
        public void DeveSelecionarMultiplasColunas()
        {
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1 });
            df.AdicionarColuna<string?>("Nome", new List<string?> { "A" });
            df.AdicionarColuna<bool?>("Ativo", new List<bool?> { true });

            var operacao = new SelecionarColuna { col = "[Id, Nome]" };
            var executor = new SelecionarColunaExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            Assert.Equal(2, resultado.Colunas.Count);
            Assert.NotNull(resultado.PegarColuna<int?>("Id"));
            Assert.NotNull(resultado.PegarColuna<string?>("Nome"));
            Assert.Null(resultado.PegarColuna<bool?>("Ativo"));
        }

        [Fact]
        public void RetornaVazioSeColunaNaoExistir()
        {
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1 });

            var operacao = new SelecionarColuna { col = "NaoExiste" };
            var executor = new SelecionarColunaExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            Assert.Empty(resultado.Colunas);
        }
    }
}
