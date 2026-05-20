using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.ManipulacaoDados;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using IntegradorDominio.Models.DataFrameModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.ManipulacaoDadosTestes
{
    public class CopiarExecutorTests
    {
        [Fact]
        public void DeveCopiarDataFrameComValoresIguais()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1, 2, 3 });
            df.AdicionarColuna<string?>("Nome", new List<string?> { "A", "B", "C" });
            df.AdicionarColuna<bool?>("Flag", new List<bool?> { true, false, true });

            var executor = new CopiarExecutor(new Copiar());

            // Act
            var resultado = (DataFrame)executor.Executar(df);

            // Assert
            Assert.Equal(df.QuantidadeLinhas, resultado.QuantidadeLinhas);
            Assert.Equal(df.Colunas.Count, resultado.Colunas.Count);

            for (int i = 0; i < df.Colunas.Count; i++)
            {
                var colOriginal = df.Colunas[i];
                var colCopiada = resultado.Colunas[i];

                Assert.Equal(colOriginal.Nome, colCopiada.Nome);
                for (int j = 0; j < df.QuantidadeLinhas; j++)
                {
                    Assert.Equal(colOriginal.PegarValor(j), colCopiada.PegarValor(j));
                }
            }
        }

        [Fact]
        public void CopiaDeveSerIndependenteDaOriginal()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna<int?>("Id", new List<int?> { 1, 2, 3 });
            df.AdicionarColuna<string?>("Nome", new List<string?> { "A", "B", "C" });

            var executor = new CopiarExecutor(new Copiar());

            // Act
            var resultado = (DataFrame)executor.Executar(df);

            // Modifica o original
            df.Colunas[0].InjetarValor(0, 999);
            df.Colunas[1].InjetarValor(1, "Z");

            // Assert: cópia não é afetada
            Assert.Equal(1, resultado.Colunas[0].PegarValor(0));
            Assert.Equal("B", resultado.Colunas[1].PegarValor(1));
        }
    }
}
