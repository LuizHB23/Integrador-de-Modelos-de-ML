using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.AgrupamentoDados;
using IntegradorDominio.FeatureEngineering.AgrupamentoDados;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.AgrupamentoDadosTestes
{
    public class MergeExecutorTests
    {
        private DataFrame CriarDataFrameEsquerdo()
        {
            var df = new DataFrame();
            df.AdicionarColuna("CustomerID", new int?[] { 1, 2, 3, 4 }.ToList());
            df.AdicionarColuna("Nome", new string?[] { "Ana", "Bruno", "Carlos", "Daniela" }.ToList());
            df.AdicionarColuna("Valor", new float?[] { 100f, 200f, 300f, 400f }.ToList());
            return df;
        }

        private DataFrame CriarDataFrameDireito()
        {
            var df = new DataFrame();
            df.AdicionarColuna("CustomerID", new int?[] { 1, 2, 4 }.ToList());
            df.AdicionarColuna("Idade", new int?[] { 25, 30, 40 }.ToList());
            df.AdicionarColuna("Cidade", new string?[] { "SP", "RJ", "MG" }.ToList());
            return df;
        }

        [Fact]
        public void Merge_SingleColumn_Sucesso()
        {
            // Arrange
            var dfEsq = CriarDataFrameEsquerdo();
            var dfDir = CriarDataFrameDireito();
            var operacao = new Merge
            {
                on = "CustomerID",
                right = "dfDireito",
                Contexto = new Dictionary<string, object>
                {
                    ["dfDireito"] = dfDir
                }
            };
            var executor = new MergeExecutor(operacao);

            // Act
            var resultado = executor.Executar(dfEsq) as DataFrame;

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(4, resultado!.QuantidadeLinhas);

            // CustomerID permanece igual
            var customerIDs = resultado.PegarColuna<int?>("CustomerID");
            Assert.Equal(new int?[] { 1, 2, 3, 4 }, customerIDs!.Dados);

            // Valores do lado direito correspondentes
            var idades = resultado.PegarColuna<int?>("Idade");
            Assert.Equal(new int?[] { 25, 30, null, 40 }, idades!.Dados);

            var cidades = resultado.PegarColuna<string?>("Cidade");
            Assert.Equal(new string?[] { "SP", "RJ", null, "MG" }, cidades!.Dados);
        }

        [Fact]
        public void Merge_ConflitoNomeColuna_Renomeia()
        {
            // Arrange
            var dfEsq = CriarDataFrameEsquerdo();
            var dfDir = new DataFrame();
            dfDir.AdicionarColuna("Valor", new float?[] { 10f, 20f, 40f }.ToList()); // mesmo nome de coluna
            dfDir.AdicionarColuna("CustomerID", new int?[] { 1, 2, 4 }.ToList());

            var operacao = new Merge
            {
                on = "CustomerID",
                right = "dfDir",
                Contexto = new Dictionary<string, object>
                {
                    ["dfDir"] = dfDir
                }
            };
            var executor = new MergeExecutor(operacao);

            // Act
            var resultado = executor.Executar(dfEsq) as DataFrame;

            // Assert
            Assert.NotNull(resultado);
            var colunaRenomeada = resultado!.PegarColuna<float?>("Valor_dfDir");
            Assert.Equal(new float?[] { 10f, 20f, null, 40f }, colunaRenomeada!.Dados);
        }

        [Fact]
        public void Merge_MultipleColumns_Sucesso()
        {
            // Arrange
            var dfEsq = new DataFrame();
            dfEsq.AdicionarColuna("CustomerID", new int?[] { 1, 1, 2 }.ToList());
            dfEsq.AdicionarColuna("Produto", new string?[] { "A", "B", "C" }.ToList());
            dfEsq.AdicionarColuna("Valor", new float?[] { 100f, 200f, 300f }.ToList());

            var dfDir = new DataFrame();
            dfDir.AdicionarColuna("CustomerID", new int?[] { 1, 1, 2 }.ToList());
            dfDir.AdicionarColuna("Produto", new string?[] { "A", "B", "C" }.ToList());
            dfDir.AdicionarColuna("Desconto", new float?[] { 10f, 20f, 30f }.ToList());

            var operacao = new Merge
            {
                on = "[CustomerID,Produto]",
                right = "dfDir",
                Contexto = new Dictionary<string, object>
                {
                    ["dfDir"] = dfDir
                }
            };
            var executor = new MergeExecutor(operacao);

            // Act
            var resultado = executor.Executar(dfEsq) as DataFrame;

            // Assert
            var descontos = resultado!.PegarColuna<float?>("Desconto");
            Assert.Equal(new float?[] { 10f, 20f, 30f }, descontos!.Dados);
        }
    }
}
