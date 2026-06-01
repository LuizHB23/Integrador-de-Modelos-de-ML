using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson.ConverteJson;
using IntegradorDominio.Models.Configuracao;
using IntegradorDominio.Models.ModeloEtapas;
using Moq;
using System.Text.Json;

namespace IntegradorTesteUnidade.AplicacaoTestes.ConversorJson
{
    public class CardsJsonTests : IDisposable
    {
        private readonly Mock<IPathProvider> _pathProviderMock;
        private readonly ConfiguradoresJson<SchemaConfiguracao> _cardsJson;
        private readonly string _tempPath;

        public CardsJsonTests()
        {
            _pathProviderMock = new Mock<IPathProvider>();

            _cardsJson = new ConfiguradoresJson<SchemaConfiguracao>();

            _tempPath = Path.Combine(Path.GetTempPath(), "CardsJsonTests");

            if (!Directory.Exists(_tempPath))
            {
                Directory.CreateDirectory(_tempPath);
            }
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
            var nomeModelo = "Nome Qualquer";

            var dados = new List<SchemaConfiguracao>()
            {
                {
                    new SchemaConfiguracao(nomeModelo, "1.0", new Dictionary<int, Schema>()
                    {
                        {
                            10,
                            new Schema()
                            {
                                NomeColuna = "Nome Coluna",
                                Finalidade = "Finalidade",
                                Tipo = "Tipo",
                                Categorico = true
                            }
                        }
                    })
                }
            };

            string json = JsonSerializer.Serialize(dados);

            string caminho = Path.Combine(_tempPath, "teste_carregar.json");

            File.WriteAllText(caminho, json);

            // Act
            var resultado = await _cardsJson.CarregarJsonAsync(caminho);

            // Assert
            Assert.Single(resultado);
            Assert.Equal(nomeModelo, dados.First().NomeModelo);
            Assert.Equal("Nome Coluna", dados.First().Colunas[10].NomeColuna);
            Assert.Equal("Finalidade", dados.First().Colunas[10].Finalidade);
            Assert.Equal("Tipo", dados.First().Colunas[10].Tipo);
            Assert.True(dados.First().Colunas[10].Categorico);
        }

        [Fact]
        public async Task NaoCriaArquivoQuandoDicionarioEstiverVazioEmConverteJsonAsync()
        {
            // Arrange
            string nomeModelo = "Nome Qualquer";

            var dados = new List<SchemaConfiguracao>();

            // Act
            await _cardsJson.ConverteJsonAsync(dados, nomeModelo);

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