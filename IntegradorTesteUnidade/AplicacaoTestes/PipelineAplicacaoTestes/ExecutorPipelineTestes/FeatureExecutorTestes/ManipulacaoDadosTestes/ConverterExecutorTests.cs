using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.ManipulacaoDados;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.ManipulacaoDadosTestes
{
    public class ConverterExecutorTests
    {
        [Fact]
        public void DeveConverterParaSingle()
        {
            var df = new DataFrame();
            df.AdicionarColuna<string?>("Valor", new List<string?> { "1.5", "2,5", null, "abc", " " });

            var executor = new ConverterExecutor(new Converter { col = "Valor", type = "single" });
            executor.Executar(df);

            var col = df.PegarColunaBase("Valor");

            Assert.Equal(1.5f, col!.PegarValor(0));
            Assert.Equal(2.5f, col.PegarValor(1));
            Assert.Null(col.PegarValor(2));
            Assert.Null(col.PegarValor(3));
            Assert.Null(col.PegarValor(4));
        }

        [Fact]
        public void DeveConverterParaBool()
        {
            var df = new DataFrame();
            df.AdicionarColuna<string?>("Flag", new List<string?> { "1", "0", "true", "false", null, "yes" });

            var executor = new ConverterExecutor(new Converter { col = "Flag", type = "bool" });
            executor.Executar(df);

            // ✅ Cast explícito para a coluna do tipo correto
            var col = df.PegarColunaBase("Flag");

            Assert.True((bool?)col.PegarValor(0));
            Assert.False((bool?)col.PegarValor(1));
            Assert.True((bool?)col.PegarValor(2));
            Assert.False((bool?)col.PegarValor(3));
            Assert.Null(col.PegarValor(4));
            Assert.Null(col.PegarValor(5));
        }

        [Fact]
        public void DeveConverterParaString()
        {
            var df = new DataFrame();
            df.AdicionarColuna<object>("Dados", new List<object> { 1, 2.5, true, null });

            var executor = new ConverterExecutor(new Converter { col = "Dados", type = "string" });
            executor.Executar(df);

            var col = df.PegarColunaBase("Dados");

            Assert.Equal("1", col!.PegarValor(0));
            Assert.Equal("2,5", col.PegarValor(1));
            Assert.Equal("True", col.PegarValor(2));
            Assert.Null(col.PegarValor(3));
        }

        [Fact]
        public void DeveConverterParaDateTime()
        {
            var df = new DataFrame();
            df.AdicionarColuna<string?>("Data", new List<string?> { "01/01/2023", "2023-02-02", "invalid", null });

            var executor = new ConverterExecutor(new Converter { col = "Data", type = "datetime" });
            executor.Executar(df);

            var col = df.PegarColunaBase("Data");

            Assert.Equal(new DateTime(2023, 1, 1), col!.PegarValor(0));
            Assert.Equal(new DateTime(2023, 2, 2), col.PegarValor(1));
            Assert.Null(col.PegarValor(2));
            Assert.Null(col.PegarValor(3));
        }

        [Fact]
        public void NaoAlteraSeColunaNaoExistir()
        {
            var df = new DataFrame();
            df.AdicionarColuna<string?>("Nome", new List<string?> { "Ana" });

            var executor = new ConverterExecutor(new Converter { col = "Idade", type = "single" });

            // Deve lançar exceção se a coluna não existir
            Assert.Throws<KeyNotFoundException>(() => executor.Executar(df));
        }
    }
}
