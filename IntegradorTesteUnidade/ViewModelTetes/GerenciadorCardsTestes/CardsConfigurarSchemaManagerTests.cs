using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson.ConverteJson;
using IntegradorDominio.Models.Configuracao;
using IntegradorDominio.Models.ModeloEtapas;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.Shared.Interfaces;
using IntegradorViewModel.Shared.Manager.GerenciadorCards;
using Moq;
using System.Collections.ObjectModel;

namespace IntegradorTesteUnidade.ViewModelTetes.GerenciadorCardsTestes
{
    public class CardsConfigurarSchemaManagerTests
    {
        private readonly ObservableCollection<ConfiguracaoCardSchemaViewModel> _cards = new();
        private readonly ObservableCollection<int> _posicoes = new();

        private Mock<IDialogService> _dialogMock;
        private Mock<IConversorJson> _converterMock;

        public CardsConfigurarSchemaManagerTests()
        {
            _dialogMock = new Mock<IDialogService>();
            _converterMock = new Mock<IConversorJson>();
        }

        private CardsConfigurarSchemaManager CriarManager()
            => new(_cards, _posicoes);

        private SchemaItemViewModel CriarItem(int pos = 1)
            => new(pos, "coluna", "target", "float", false);

        private ConfiguracaoCardSchemaViewModel CriarCard(int pos = 1)
            => new(
                CriarItem(pos),
                _ => Task.CompletedTask,
                (_, __) => Task.CompletedTask
            );

        [Fact]
        public void AdicionarColuna_DeveAdicionarCard()
        {
            //Arrange
            var manager = CriarManager();

            //Act
            manager.AdicinarColuna(CriarItem(), _ => Task.CompletedTask, (_, __) => Task.CompletedTask);

            //Assert
            Assert.Single(_cards);
            Assert.Single(_posicoes);
        }

        [Fact]
        public void AdicionarColuna_NaoAdiciona_QuandoNull()
        {
            //Arrange
            var manager = CriarManager();

            //Act
            manager.AdicinarColuna(null, _ => Task.CompletedTask, (_, __) => Task.CompletedTask);

            //Assert
            Assert.Empty(_cards);
        }

        [Fact]
        public async Task CarregarSchema_DevePopularLista()
        {
            //Arrange
            var manager = CriarManager();

            _dialogMock.Setup(x => x.GetCaminhoArquivo()).Returns("fake.json");

            _converterMock.Setup(x => x.CarregarJsonAsync<List<SchemaConfiguracao>>(It.IsAny<string>())).ReturnsAsync(new List<SchemaConfiguracao>
                {
                    { new SchemaConfiguracao("Modelo", "1.0", new Dictionary<int, Schema>()
                        {
                            {
                                1,
                                new Schema { NomeColuna = "col1", Finalidade = "target", Tipo = "Single", Categorico = false }
                            }
                        }) 
                    }
                });

            //Act
            await manager.CarregarSchema(_dialogMock.Object, _converterMock.Object);

            //Assert
            Assert.Single(_cards);
            Assert.Single(_posicoes);
        }

        [Fact]
        public async Task CarregarSchema_NaoFazNada_QuandoCaminhoVazio()
        {
            //Arrange
            var manager = CriarManager();

            _dialogMock.Setup(x => x.GetCaminhoArquivo()).Returns("");

            //Act
            await manager.CarregarSchema(_dialogMock.Object, _converterMock.Object);

            //Assert
            Assert.Empty(_cards);
        }

        [Fact]
        public async Task PreparaParaJson_DeveChamarConversao()
        {
            //Arrange
            string nomeModelo = "modelo";
            var manager = CriarManager();

            _cards.Add(CriarCard());

            //Act
            await manager.PreparaParaJson(_converterMock.Object, nomeModelo);

            //Assert
            _converterMock.Verify(x =>
                x.ConverteJsonAsync(It.IsAny<List<SchemaConfiguracao>>(), nomeModelo),
                Times.Once);
        }
    }
}
