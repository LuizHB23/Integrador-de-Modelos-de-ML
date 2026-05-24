using IntegradorAplicacao.DTO;
using IntegradorAplicacao.DTO.Interfaces;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson.ConverteJson;
using Moq;
using System.Text.Json;

namespace IntegradorTesteUnidade.AplicacaoTestes.ConversorJson
{
    public class CardsJsonTests : IDisposable
    {
        private readonly Mock<IPathProvider> _pathProviderMock;
        private readonly CardsJson<SchemaDTO> _cardsJson;
        private readonly string _tempPath;

        public CardsJsonTests()
        {
            _pathProviderMock = new Mock<IPathProvider>();

            _cardsJson = new CardsJson<SchemaDTO>(_pathProviderMock.Object);

            _tempPath = Path.Combine(Path.GetTempPath(), "CardsJsonTests");

            if (!Directory.Exists(_tempPath))
            {
                Directory.CreateDirectory(_tempPath);
            }
        }

        [Fact]
        public async Task UsaCaminhoSchemaQuandoTipoForSchemaDTO()
        {
            // Arrange
            var dados = new Dictionary<int, SchemaDTO>
                {
                    {
                        1,
                        new SchemaDTO("Coluna", "Finalidade", "Tipo", true)
                        {
                            NomeModelo = "ModeloTeste"
                        }
                    }
                };

            string caminho = Path.Combine(_tempPath, "schema.json");

            _pathProviderMock
                .Setup(p => p.GetCaminhoSchemaConfig("ModeloTeste"))
                .Returns(caminho);

            var conversor = new CardsJson<SchemaDTO>(_pathProviderMock.Object);

            // Act
            await conversor.ConverteJsonAsync(dados);

            // Assert
            _pathProviderMock.Verify(
                p => p.GetCaminhoSchemaConfig("ModeloTeste"),
                Times.Once);
        }

        [Fact]
        public async Task UsaCaminhoPipelineQuandoTipoForFuncaoDTO()
        {
            // Arrange
            var dados = new Dictionary<int, FuncaoDTO>
                {
                    {
                        1,
                        new FuncaoDTO()
                        {
                            NomeModelo = "ModeloTeste"
                        }
                    }
                };

            string caminho = Path.Combine(_tempPath, "pipeline.json");

            _pathProviderMock
                .Setup(p => p.GetCaminhoPipelineConfig("ModeloTeste"))
                .Returns(caminho);

            var conversor = new CardsJson<FuncaoDTO>(_pathProviderMock.Object);

            // Act
            await conversor.ConverteJsonAsync(dados);

            // Assert
            _pathProviderMock.Verify(
                p => p.GetCaminhoPipelineConfig("ModeloTeste"),
                Times.Once);
        }

        [Fact]
        public async Task UsaCaminhoTransformadorQuandoTipoForTransformadorDTO()
        {
            // Arrange
            var dados = new Dictionary<int, TransformadorDTO>
                {
                    {
                        1,
                        new TransformadorDTO("", "")
                        {
                            NomeModelo = "ModeloTeste"
                        }
                    }
                };

            string caminho = Path.Combine(_tempPath, "transformador.json");

            _pathProviderMock
                .Setup(p => p.GetCaminhoTransformadorConfig("ModeloTeste"))
                .Returns(caminho);

            var conversor = new CardsJson<TransformadorDTO>(_pathProviderMock.Object);

            // Act
            await conversor.ConverteJsonAsync(dados);

            // Assert
            _pathProviderMock.Verify(
                p => p.GetCaminhoTransformadorConfig("ModeloTeste"),
                Times.Once);
        }

        [Fact]
        public async Task UsaCaminhoSaidaQuandoTipoForSaidaDTO()
        {
            // Arrange
            var dados = new Dictionary<int, SaidaDTO>
                {
                    {
                        1,
                        new SaidaDTO()
                        {
                            NomeModelo = "ModeloTeste"
                        }
                    }
                };

            string caminho = Path.Combine(_tempPath, "saida.json");

            _pathProviderMock
                .Setup(p => p.GetCaminhoSaidaConfig("ModeloTeste"))
                .Returns(caminho);

            var conversor = new CardsJson<SaidaDTO>(_pathProviderMock.Object);

            // Act
            await conversor.ConverteJsonAsync(dados);

            // Assert
            _pathProviderMock.Verify(
                p => p.GetCaminhoSaidaConfig("ModeloTeste"),
                Times.Once);
        }

        [Fact]
        public async Task RetornaDicionarioVazioSeArquivoNaoExisteEmCarregarJsonAsync()
        {
            // Arrange
            string caminho = Path.Combine(_tempPath, "arquivo_inexistente.json");

            // Act
            var resultado = await _cardsJson.CarregarJsonAsync(caminho);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        [Fact]
        public async Task RetornaDicionarioPreenchidoSeArquivoValidoEmCarregarJsonAsync()
        {
            // Arrange
            var dados = new Dictionary<int, SchemaDTO>
            {
                {
                    10,
                    new SchemaDTO(
                        "Nome Coluna Qualquer",
                        "Finalidade Qualquer",
                        "Tipo Qualquer",
                        true)
                    {
                        NomeModelo = "Nome Modelo Qualquer"
                    }
                }
            };

            string json = JsonSerializer.Serialize(dados);

            string caminho = Path.Combine(_tempPath, "teste_carregar.json");

            File.WriteAllText(caminho, json);

            // Act
            var resultado = await _cardsJson.CarregarJsonAsync(caminho);

            // Assert
            Assert.Single(resultado);

            Assert.Equal(
                "Nome Modelo Qualquer",
                resultado[10].NomeModelo);
        }

        [Fact]
        public async Task NaoCriaArquivoQuandoDicionarioEstiverVazioEmConverteJsonAsync()
        {
            // Arrange
            var dados = new Dictionary<int, SchemaDTO>();

            // Act
            await _cardsJson.ConverteJsonAsync(dados);

            // Assert
            Assert.Empty(Directory.GetFiles(_tempPath));
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