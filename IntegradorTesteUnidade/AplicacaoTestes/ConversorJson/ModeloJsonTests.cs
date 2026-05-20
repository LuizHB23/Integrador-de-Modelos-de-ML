using Xunit;
using Moq;
using IntegradorAplicacao.DTO;
using System.IO;
using System.Text.Json;
using IntegradorAplicacao.Infraestrutura.ConversorJSON;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;

namespace IntegradorAplicacao.Tests
{
    public class ModeloJsonTests : IDisposable
    {
        private readonly Mock<IPathProvider> _pathProviderMock;
        private readonly ModeloJson _modeloJson;
        private readonly string _basePath;

        public ModeloJsonTests()
        {
            _pathProviderMock = new Mock<IPathProvider>();
            _modeloJson = new ModeloJson(_pathProviderMock.Object);

            // Pasta temporária para o teste não sujar seu PC
            _basePath = Path.Combine(Path.GetTempPath(), "ModeloJsonTests");
            if (!Directory.Exists(_basePath)) Directory.CreateDirectory(_basePath);
        }

        [Fact]
        public void RetornaEhVerdadeiroEContemAoSalvarArquivoModeloNoCaminhoCorretoEmConverteJson()
        {
            // Arrange
            var modelo = new ModeloDTO("Nome Qualquer", "Tipo Qualquer", "Caminho Qualquer");
            _pathProviderMock.Setup(p => p.GetCaminhoModelo()).Returns(_basePath);

            string pastaModelo = Path.Combine(_basePath, modelo.NomeModelo);
            Directory.CreateDirectory(pastaModelo); // Necessário para o File.WriteAllText não quebrar
            string caminhoEsperado = Path.Combine(pastaModelo, "modelo.json");

            // Act
            _modeloJson.ConverteJson(modelo);

            // Assert
            Assert.True(File.Exists(caminhoEsperado));
            var jsonSalvo = File.ReadAllText(caminhoEsperado);
            Assert.Contains("Nome Qualquer", jsonSalvo);
        }

        public void Dispose()
        {
            if (Directory.Exists(_basePath)) Directory.Delete(_basePath, true);
        }
    }
}