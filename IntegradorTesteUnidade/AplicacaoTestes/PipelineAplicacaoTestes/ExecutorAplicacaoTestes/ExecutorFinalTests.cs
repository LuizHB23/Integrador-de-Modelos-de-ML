using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.PipelineAplicacao.ExecutorAplicacao;
using IntegradorAplicacao.PipelineAplicacao.ParserPipeline;
using IntegradorDominio.AST;
using IntegradorDominio.DataFrameModel;
using Moq;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorAplicacaoTestes
{
    public class ExecutorFinalTests
    {
        [Fact]
        public void ConstroiSequenciaMetodoPipeline_DeveConstruirExecutors()
        {
            var mockConversor = new Mock<IConverteJson<Dictionary<int, FuncaoDTO>>>();
            var mockParser = new ParserAst(); // parser real para simplificar

            var funcaoDto = new FuncaoDTO("teste", new List<string> { "df = df.DropDuplicates()" }, "");

            mockConversor.Setup(c => c.CarregarJson(It.IsAny<string>()))
                         .Returns(new Dictionary<int, FuncaoDTO> { { 1, funcaoDto } });

            var executor = new ExecutorFinal(mockConversor.Object);

            // Não temos acesso ao parser interno, mas a ideia é testar que não explode
            executor.ConstroiSequenciaMetodoPipeline("caminho/fake.json");
        }

        [Fact]
        public void ExecutarTudo_DeveRetornarDataFrameMesmoSeSemMetodos()
        {
            var mockConversor = new Mock<IConverteJson<Dictionary<int, FuncaoDTO>>>();
            var executor = new ExecutorFinal(mockConversor.Object);

            var dfOriginal = new DataFrame();
            dfOriginal.NomeContexto = "dfOriginal";

            var resultado = executor.ExecutarTudo(dfOriginal);

            Assert.NotNull(resultado);
            Assert.Equal("dfOriginal", resultado.NomeContexto);
        }

        [Fact]
        public void RecuperaMetodoPipeline_DeveChamarConversorECriarListaMetodo()
        {
            var mockConversor = new Mock<IConverteJson<Dictionary<int, FuncaoDTO>>>();
            var parser = new ParserAst();

            var funcaoDto = new FuncaoDTO("f", new List<string> { "df = df.DropDuplicates()" }, "");

            mockConversor.Setup(c => c.CarregarJson(It.IsAny<string>()))
                         .Returns(new Dictionary<int, FuncaoDTO> { { 1, funcaoDto } });

            var executor = new ExecutorFinal(mockConversor.Object);

            // Usando reflection para testar método privado (opcional)
            var metodoPrivado = typeof(ExecutorFinal).GetMethod(
                "RecuperaMetodoPipeline",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );

            var result = (List<MetodoPipeline>)metodoPrivado.Invoke(executor, new object[] { "caminho/fake.json" })!;

            Assert.Single(result);
            Assert.NotNull(result[0]);
        }
    }
}
