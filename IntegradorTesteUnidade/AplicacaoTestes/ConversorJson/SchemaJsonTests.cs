using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using System.Text.Json;
using Moq;

namespace IntegradorTesteUnidade.AplicacaoTestes.ConversorJson
{
    public class SchemaJsonTests : IDisposable
    {
        private readonly Mock<IPathProvider> _pathProviderMock;
        private readonly SchemaJson _schemaJson;
        private readonly string _tempPath;

        public SchemaJsonTests()
        {
            _pathProviderMock = new Mock<IPathProvider>();
            _schemaJson = new SchemaJson(_pathProviderMock.Object);

            // Criamos uma pasta temporária para os testes
            _tempPath = Path.Combine(Path.GetTempPath(), "SchemaJsonTests");
            if (!Directory.Exists(_tempPath)) Directory.CreateDirectory(_tempPath);
        }

        [Fact]
        public void RetornaVerdadeiroEContemQuandoCirarArquvioComDadosSerializadosEmConverteJson()
        {
            //Arrange
            var schemaNovo = new Dictionary<int, SchemaDTO>
            {
                {1, new SchemaDTO("Nome Coluna Qualquer", "Finalidade Qualquer", "Tipo Qualquer", true, "Nome Modelo Qualquer")}
            };

            _pathProviderMock.Setup(p => p.GetCaminhoModelo()).Returns(_tempPath);

            string subPasta = Path.Combine(_tempPath, "Nome Modelo Qualquer");
            Directory.CreateDirectory(subPasta);
            string caminhoEsperado = Path.Combine(subPasta, "schema.json");

            //Act
            _schemaJson.ConverteJson(schemaNovo);

            //Assert
            Assert.True(File.Exists(caminhoEsperado));
            var conteudo = File.ReadAllText(caminhoEsperado);
            Assert.Contains("Nome Modelo Qualquer", conteudo);
        }

        [Fact]
        public void RetornaDicionarioVazioSeArquivoNaoExisteEmCarregarJson()
        {
            //Arrange + Act
            var resultado = _schemaJson.CarregarJson("caminho_inexistente.json");

            //Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        [Fact]
        public void RetornaDicionarioPreenchidoSeArquivoValidoemCarregarJson()
        {
            //Arrange
            var dados = new Dictionary<int, SchemaDTO>
            {
                {10, new SchemaDTO("Nome Coluna Qualquer", "Finalidade Qualquer", "Tipo Qualquer", true, "Nome Modelo Qualquer")}
            };
            string json = JsonSerializer.Serialize(dados);
            string caminho = Path.Combine(_tempPath, "teste_carregar.json");
            File.WriteAllText(caminho, json);

            //Act
            var resultado = _schemaJson.CarregarJson(caminho);

            //Assert
            Assert.Single(resultado);
            Assert.Equal("Nome Modelo Qualquer", resultado[10].NomeModelo);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempPath))
            {
                Directory.Delete(_tempPath, true);
            }
        }
    }
}
