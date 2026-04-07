using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.AgrupamentoDados;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.AgrupamentoDados;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.AgrupamentoDadosTestes
{
    public class SortExecutorTests
    {
        private DataFrame CriarDataFrameExemplo()
        {
            var df = new DataFrame();
            df.AdicionarColuna("CustomerID", new int?[] { 3, 1, 2, 4 }.ToList());
            df.AdicionarColuna("Nome", new string?[] { "Carlos", "Ana", "Bruno", null }.ToList());
            df.AdicionarColuna("Valor", new float?[] { 300f, 100f, 200f, 400f }.ToList());
            return df;
        }

        [Fact]
        public void Sort_SingleColumn_Ascendente()
        {
            var df = CriarDataFrameExemplo();
            var operacao = new Sort
            {
                col = "CustomerID",
                asc = "true"
            };
            var executor = new SortExecutor(operacao);

            var resultado = executor.Executar(df) as DataFrame;

            var ids = resultado!.PegarColuna<int?>("CustomerID");
            Assert.Equal(new int?[] { 1, 2, 3, 4 }, ids!.Dados);

            var nomes = resultado.PegarColuna<string?>("Nome");
            Assert.Equal(new string?[] { "Ana", "Bruno", "Carlos", null }, nomes!.Dados);
        }

        [Fact]
        public void Sort_SingleColumn_Descendente()
        {
            var df = CriarDataFrameExemplo();
            var operacao = new Sort
            {
                col = "CustomerID",
                asc = "false"
            };
            var executor = new SortExecutor(operacao);

            var resultado = executor.Executar(df) as DataFrame;

            var ids = resultado!.PegarColuna<int?>("CustomerID");
            Assert.Equal(new int?[] { 4, 3, 2, 1 }, ids!.Dados);
        }

        [Fact]
        public void Sort_MultipleColumns()
        {
            var df = new DataFrame();
            df.AdicionarColuna("CustomerID", new int?[] { 1, 1, 2, 2 }.ToList());
            df.AdicionarColuna("Valor", new float?[] { 200f, 100f, 300f, 100f }.ToList());

            var operacao = new Sort
            {
                col = "[CustomerID,Valor]",
                asc = "true"
            };
            var executor = new SortExecutor(operacao);

            var resultado = executor.Executar(df) as DataFrame;

            var valores = resultado!.PegarColuna<float?>("Valor");
            Assert.Equal(new float?[] { 100f, 200f, 100f, 300f }, valores!.Dados);
        }

        [Fact]
        public void Sort_NulosSempreNoFim()
        {
            var df = CriarDataFrameExemplo();
            var operacao = new Sort
            {
                col = "Nome",
                asc = "true"
            };
            var executor = new SortExecutor(operacao);

            var resultado = executor.Executar(df) as DataFrame;

            var nomes = resultado!.PegarColuna<string?>("Nome");
            Assert.Equal(new string?[] { "Ana", "Bruno", "Carlos", null }, nomes!.Dados);
        }
    }
}
