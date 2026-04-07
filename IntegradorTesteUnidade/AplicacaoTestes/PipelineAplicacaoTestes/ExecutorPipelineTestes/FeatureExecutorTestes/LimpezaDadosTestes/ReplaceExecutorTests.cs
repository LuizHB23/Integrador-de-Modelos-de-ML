using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.LimpezaDados;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.LimpezaDados;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.LimpezaDadosTestes
{
    public class ReplaceExecutorTests
    {
        [Fact]
        public void DeveSubstituirString()
        {
            var df = new DataFrame();
            df.AdicionarColuna<string?>("Nome", new List<string?> { "Ana", "Beto", "Cleo" });

            var executor = new ReplaceExecutor(new Replace { col = "Nome", old = "Beto", value = "Bruno" });
            executor.Executar(df);

            Assert.Equal("Ana", df.PegarColunaBase("Nome")!.PegarValor(0));
            Assert.Equal("Bruno", df.PegarColunaBase("Nome")!.PegarValor(1));
            Assert.Equal("Cleo", df.PegarColunaBase("Nome")!.PegarValor(2));
        }

        [Fact]
        public void DeveSubstituirNumerico()
        {
            var df = new DataFrame();
            df.AdicionarColuna<float?>("Idade", new List<float?> { 10f, 20f, 30f });

            var executor = new ReplaceExecutor(new Replace { col = "Idade", old = "20", value = "25" });
            executor.Executar(df);

            Assert.Equal(10f, df.PegarColunaBase("Idade")!.PegarValor(0));
            Assert.Equal(25f, df.PegarColunaBase("Idade")!.PegarValor(1));
            Assert.Equal(30f, df.PegarColunaBase("Idade")!.PegarValor(2));
        }

        [Fact]
        public void DeveSubstituirBooleano()
        {
            var df = new DataFrame();
            df.AdicionarColuna<bool?>("Ativo", new List<bool?> { true, false, true });

            var executor = new ReplaceExecutor(new Replace { col = "Ativo", old = "false", value = "true" });
            executor.Executar(df);

            Assert.Equal(true, df.PegarColunaBase("Ativo")!.PegarValor(0));
            Assert.Equal(true, df.PegarColunaBase("Ativo")!.PegarValor(1));
            Assert.Equal(true, df.PegarColunaBase("Ativo")!.PegarValor(2));
        }

        [Fact]
        public void DeveSubstituirData()
        {
            var df = new DataFrame();
            df.AdicionarColuna<DateTime?>("Nascimento", new List<DateTime?> { new DateTime(2000, 1, 1), new DateTime(2010, 2, 2) });

            var executor = new ReplaceExecutor(new Replace { col = "Nascimento", old = "2010-02-02", value = "2012-03-03" });
            executor.Executar(df);

            Assert.Equal(new DateTime(2000, 1, 1), df.PegarColunaBase("Nascimento")!.PegarValor(0));
            Assert.Equal(new DateTime(2012, 3, 3), df.PegarColunaBase("Nascimento")!.PegarValor(1));
        }

        [Fact]
        public void DeveLancarExcecaoSeColunaNaoExistir()
        {
            var df = new DataFrame();
            df.AdicionarColuna<string?>("Nome", new List<string?> { "Ana" });

            var executor = new ReplaceExecutor(new Replace { col = "Idade", old = "20", value = "25" });

            Assert.Throws<Exception>(() => executor.Executar(df));
        }

        [Fact]
        public void NaoSubstituiSeValorNaoExistir()
        {
            var df = new DataFrame();
            df.AdicionarColuna<string?>("Nome", new List<string?> { "Ana", "Beto" });

            var executor = new ReplaceExecutor(new Replace { col = "Nome", old = "Cleo", value = "Bruno" });
            executor.Executar(df);

            Assert.Equal("Ana", df.PegarColunaBase("Nome")!.PegarValor(0));
            Assert.Equal("Beto", df.PegarColunaBase("Nome")!.PegarValor(1));
        }
    }
}
