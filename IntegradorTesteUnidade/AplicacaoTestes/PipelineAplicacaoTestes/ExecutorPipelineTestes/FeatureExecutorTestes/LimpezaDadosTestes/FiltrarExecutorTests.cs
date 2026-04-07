using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.LimpezaDados;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.LimpezaDados;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.LimpezaDadosTestes
{
    public class FiltrarExecutorTests
    {
        [Fact]
        public void FiltrarExecutor_DeveFiltrarLinhasCorretamente()
        {
            // Arrange: criar DataFrame com diferentes tipos de colunas
            var df = new DataFrame();
            df.AdicionarColuna<float?>("ValorFloat", new List<float?> { 5f, 10f, 15f, null });
            df.AdicionarColuna<bool?>("ValorBool", new List<bool?> { true, false, true, null });
            df.AdicionarColuna<string?>("ValorString", new List<string?> { "A", "B", "C", "D" });

            // Operação: filtrar linhas onde ValorFloat > 5 && ValorBool == true
            var operacao = new Filtrar
            {
                condition = "ValorFloat > 5 && ValorBool == true"
            };
            var executor = new FiltrarExecutor(operacao);

            // Act
            var resultado = (DataFrame)executor.Executar(df);

            // Assert
            var colunaFloat = resultado.PegarColunaBase("ValorFloat") as Coluna<float?>;
            var colunaBool = resultado.PegarColunaBase("ValorBool") as Coluna<bool?>;
            var colunaString = resultado.PegarColunaBase("ValorString") as Coluna<string?>;

            // Apenas a linha índice 2 (ValorFloat=15, ValorBool=true) deve passar
            Assert.Equal(new float?[] { 15f }, colunaFloat!.Dados);
            Assert.Equal(new bool?[] { true }, colunaBool!.Dados);
            Assert.Equal(new string?[] { "C" }, colunaString!.Dados);
        }
    }
}
