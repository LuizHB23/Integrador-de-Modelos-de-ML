using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.PipelineAplicacao.ParserPipeline;
using IntegradorDominio.AST;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ParserPipelineTestes
{
    public class JsonToParserTests
    {
        [Fact]
        public void EnviaMetodoPipeline_DeveRetornarMetodoPipeline()
        {
            // Arrange
            var mockConversor = new Mock<IConverteJson<Dictionary<int, FuncaoDTO>>>();
            var mockProvider = new Mock<IPathProvider>();

            var funcaoDto = new FuncaoDTO("TesteFuncao", new List<string> { "x = y", "return x" }, "");

            mockConversor.Setup(c => c.CarregarJson(It.IsAny<string>()))
                         .Returns(new Dictionary<int, FuncaoDTO> { { 1, funcaoDto } });

            var parser = new JsonToParser(mockConversor.Object, mockProvider.Object);

            // Act
            var metodoPipeline = parser.EnviaMetodoPipeline("caminho/fake.json");

            // Assert
            Assert.NotNull(metodoPipeline);
            Assert.Equal("TesteFuncao", metodoPipeline.Nome);
            Assert.Equal(2, metodoPipeline.Comandos.Count);
            Assert.IsType<AtribuicaoMetodoPipeline>(metodoPipeline.Comandos[0]);
            Assert.IsType<RetornoMetodoPipeline>(metodoPipeline.Comandos[1]);
        }

        [Fact]
        public void CarregarCodigos_DeveTransformarJsonEmDicionarioNomeCorpo()
        {
            // Arrange
            var mockConversor = new Mock<IConverteJson<Dictionary<int, FuncaoDTO>>>();
            var mockProvider = new Mock<IPathProvider>();

            var funcaoDto1 = new FuncaoDTO("Funcao1", new List<string> { "return 1" }, "");

            var funcaoDto2 = new FuncaoDTO("Funcao2", new List<string> { "x = 2", "return x" }, "");

            mockConversor.Setup(c => c.CarregarJson(It.IsAny<string>()))
                         .Returns(new Dictionary<int, FuncaoDTO>
                         {
                             { 1, funcaoDto1 },
                             { 2, funcaoDto2 }
                         });

            var parser = new JsonToParser(mockConversor.Object, mockProvider.Object);

            // Use reflection para chamar método privado CarregarCodigos
            var metodoPrivado = typeof(JsonToParser)
                .GetMethod("CarregarCodigos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var result = (Dictionary<string, List<string>>)metodoPrivado.Invoke(parser, new object[] { "caminho/fake.json" })!;

            // Assert
            Assert.Equal(2, result.Count);
            Assert.True(result.ContainsKey("Funcao1"));
            Assert.True(result.ContainsKey("Funcao2"));
            Assert.Equal(funcaoDto1.Codigo, result["Funcao1"]);
            Assert.Equal(funcaoDto2.Codigo, result["Funcao2"]);
        }
    }
}
