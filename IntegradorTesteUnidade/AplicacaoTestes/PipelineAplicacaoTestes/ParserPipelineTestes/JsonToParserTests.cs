using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ParserPipeline;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorDominio.AST;
using Moq;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ParserPipelineTestes
{
    public class JsonToParserTests
    {
        private Mock<IConversorJson> _mockConversor;
        private Mock<IPathProvider> _mockProvider;

        public JsonToParserTests()
        {
             _mockConversor = new Mock<IConversorJson>();
             _mockProvider = new Mock<IPathProvider>();
        }

        [Fact]
        public async Task EnviaMetodoPipeline_DeveRetornarMetodoPipeline()
        {
            // Arrange
            var funcaoDto = new FuncaoDTO
            {
                NomeFuncao = "TesteFuncao", 
                Codigo = new List<string> { "x = y", "return x" }, 
                NomeModelo = ""
            };

            _mockConversor.Setup(c => c.CarregarJsonAsync<Dictionary<int, FuncaoDTO>>(It.IsAny<string>())).ReturnsAsync(new Dictionary<int, FuncaoDTO> { { 1, funcaoDto } });

            var parser = new JsonToParser(_mockConversor.Object, _mockProvider.Object);

            // Act
            var metodoPipeline = await parser.EnviaMetodoPipeline("caminho/fake.json");

            // Assert
            Assert.NotNull(metodoPipeline);
            Assert.Equal("TesteFuncao", metodoPipeline.Nome);
            Assert.Equal(2, metodoPipeline.Comandos.Count);
            Assert.IsType<AtribuicaoMetodoPipeline>(metodoPipeline.Comandos[0]);
            Assert.IsType<RetornoMetodoPipeline>(metodoPipeline.Comandos[1]);
        }

        [Fact]
        public async Task CarregarCodigos_DeveTransformarJsonEmDicionarioNomeCorpo()
        {
            // Arrange
            var funcaoDto1 = new FuncaoDTO
            {
                NomeFuncao = "Funcao1",
                Codigo = new List<string> { "return 1" },
                NomeModelo = ""
            };

            var funcaoDto2 = new FuncaoDTO
            {
                NomeFuncao = "Funcao2",
                Codigo = new List<string> { "x = 2", "return x" },
                NomeModelo = ""
            };

            _mockConversor.Setup(c => c.CarregarJsonAsync<Dictionary<int, FuncaoDTO>>(It.IsAny<string>())).ReturnsAsync(new Dictionary<int, FuncaoDTO>
                         {
                             { 1, funcaoDto1 },
                             { 2, funcaoDto2 }
                         });

            var parser = new JsonToParser(_mockConversor.Object, _mockProvider.Object);

            // Use reflection para chamar método privado CarregarCodigos
            var metodoPrivado = typeof(JsonToParser)
                .GetMethod("CarregarCodigos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var result = await (Task<Dictionary<string, List<string>>>)metodoPrivado.Invoke(parser, new object[] { "caminho/fake.json" })!;

            // Assert
            Assert.Equal(2, result.Count);
            Assert.True(result.ContainsKey("Funcao1"));
            Assert.True(result.ContainsKey("Funcao2"));
            Assert.Equal(funcaoDto1.Codigo, result["Funcao1"]);
            Assert.Equal(funcaoDto2.Codigo, result["Funcao2"]);
        }
    }
}
