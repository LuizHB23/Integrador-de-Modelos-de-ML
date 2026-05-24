using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorAplicacao;
using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ParserPipeline;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson.ConverteJson;
using IntegradorDominio.AST;
using IntegradorDominio.Models.DataFrameModel;
using Moq;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorAplicacaoTestes
{
    public class ExecutorFinalTests
    {
        private readonly Mock<IConversorJson> _mockConversor;

        public ExecutorFinalTests()
        {
            _mockConversor = new Mock<IConversorJson>();
        }

        [Fact]
        public async Task ConstroiSequenciaMetodoPipeline_DeveConstruirExecutors()
        {
            var funcaoDto = new FuncaoDTO
            {
                NomeFuncao = "teste",
                Codigo = new List<string> { "df = df.DropDuplicates()" },
                NomeModelo = ""
            };

            _mockConversor.Setup(c => c.CarregarJsonAsync<Dictionary<int, FuncaoDTO>>(It.IsAny<string>())).ReturnsAsync(new Dictionary<int, FuncaoDTO> { { 1, funcaoDto } });

            var executor = new ExecutorFinal<FuncaoDTO>(_mockConversor.Object);

            // Não temos acesso ao parser interno, mas a ideia é testar que não explode
            await executor.ConstroiSequenciaMetodoPipeline("caminho/fake.json");
        }

        [Fact]
        public void ExecutarTudo_DeveRetornarDataFrameMesmoSeSemMetodos()
        {
            var _mockConversor = new Mock<IConversorJson>();
            var executor = new ExecutorFinal<FuncaoDTO>(_mockConversor.Object);

            var dfOriginal = new DataFrame();
            dfOriginal.NomeContexto = "dfOriginal";

            var resultado = executor.ExecutarTudo(dfOriginal);

            Assert.NotNull(resultado);
            Assert.Equal("dfOriginal", resultado.NomeContexto);
        }

        [Fact]
        public async Task RecuperaMetodoPipeline_DeveChamarConversorECriarListaMetodo()
        {
            var funcaoDto = new FuncaoDTO
            {
                NomeFuncao = "f",
                Codigo = new List<string> { "df = df.DropDuplicates()" },
                NomeModelo = ""
            };

            _mockConversor.Setup(c => c.CarregarJsonAsync<Dictionary<int, FuncaoDTO>>(It.IsAny<string>())).ReturnsAsync(new Dictionary<int, FuncaoDTO> { { 1, funcaoDto } });

            var executor = new ExecutorFinal<FuncaoDTO>(_mockConversor.Object);

            // Usando reflection para testar método privado (opcional)
            var metodoPrivado = typeof(ExecutorFinal<FuncaoDTO>).GetMethod(
                "RecuperaMetodoPipeline",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );

            var result = await (Task<List<MetodoPipeline>>)metodoPrivado.Invoke(executor, new object[] { "caminho/fake.json" })!;

            Assert.Single(result);
            Assert.NotNull(result[0]);
        }
    }
}
