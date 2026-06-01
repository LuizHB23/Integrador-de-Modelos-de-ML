using IntegradorDominio.Models.Enums;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorDominio.Models.Configuracao;
using Moq;

namespace IntegradorAplicacao.Tests
{
    public class ModeloJsonTests : IDisposable
    {
        private readonly Mock<IPathProvider> _pathProviderMock;
        private readonly IConversorJson _modeloJson;
        private readonly string _basePath;

        public ModeloJsonTests()
        {
            _pathProviderMock = new Mock<IPathProvider>();
            _modeloJson = new ConversorJson(_pathProviderMock.Object);

            // Pasta temporária para o teste não sujar seu PC
            _basePath = Path.Combine(Path.GetTempPath(), "ModeloJsonTests", "config");
            if (!Directory.Exists(_basePath)) Directory.CreateDirectory(_basePath);
        }

        [Fact]
        public async Task RetornaEhVerdadeiroEContemAoSalvarArquivoModeloNoCaminhoCorretoEmConverteJson()
        {
            // Arrange
            string nomeModelo = "Nome Qualquer";

            var modelo = new ModeloEmUsoConfiguracao(nomeModelo, TipoModelo.Classificao, "Caminho Qualquer");

            string pastaDoModelo = Path.Combine(_basePath, modelo.NomeModelo, "config");
            string caminhoEsperadoDoArquivo = Path.Combine(pastaDoModelo, "modelo.json");

            Directory.CreateDirectory(pastaDoModelo);

            _pathProviderMock.Setup(p => p.GetCaminhoModeloEmUsoConfig(nomeModelo))
                             .Returns(caminhoEsperadoDoArquivo);

            // Act
            await _modeloJson.ConverteJsonAsync(modelo, nomeModelo);

            // Assert
            Assert.True(File.Exists(caminhoEsperadoDoArquivo));
            var jsonSalvo = File.ReadAllText(caminhoEsperadoDoArquivo);
            Assert.Contains("Nome Qualquer", jsonSalvo);
        }

        public void Dispose()
        {
            if (Directory.Exists(_basePath)) Directory.Delete(_basePath, true);
        }
    }
}