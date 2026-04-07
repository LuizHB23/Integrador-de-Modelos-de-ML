using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.ManipulacaoDados;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.ManipulacaoDadosTestes
{
    public class RenomearColunaExecutorTests
    {
        [Fact]
        public void DeveRenomearUmaColuna()
        {
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1, 2, 3 });
            df.AdicionarColuna<string?>("Nome", new List<string?> { "A", "B", "C" });

            var operacao = new RenomearColuna { col = "Id", name = "Identificador" };
            var executor = new RenomearColunaExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            Assert.NotNull(resultado.PegarColuna<int?>("Identificador"));
            Assert.Null(resultado.PegarColuna<int?>("Id"));
            Assert.NotNull(resultado.PegarColuna<string?>("Nome"));
        }

        [Fact]
        public void DeveRenomearMultiplasColunas()
        {
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1 });
            df.AdicionarColuna<string?>("Nome", new List<string?> { "A" });

            var operacao = new RenomearColuna
            {
                col = "[Id, Nome]",
                name = "[Identificador, NomeCompleto]"
            };
            var executor = new RenomearColunaExecutor(operacao);

            var resultado = (DataFrame)executor.Executar(df);

            Assert.NotNull(resultado.PegarColuna<int?>("Identificador"));
            Assert.NotNull(resultado.PegarColuna<string?>("NomeCompleto"));
            Assert.Null(resultado.PegarColuna<int?>("Id"));
            Assert.Null(resultado.PegarColuna<string?>("Nome"));
        }

        [Fact]
        public void JogaExecaoSeColunaNaoExistir()
        {
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1 });

            var operacao = new RenomearColuna { col = "NaoExiste", name = "NovoNome" };
            var executor = new RenomearColunaExecutor(operacao);

            Assert.Throws<Exception>(() => executor.Executar(df));
        }
    }
}
