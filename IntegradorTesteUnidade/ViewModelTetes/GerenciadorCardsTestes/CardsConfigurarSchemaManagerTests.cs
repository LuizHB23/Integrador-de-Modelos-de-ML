using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson.ConverteJson;
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
                _ => { },
                (_, __) => { }
            );

        [Fact]
        public void AdicionarColuna_DeveAdicionarCard()
        {
            var manager = CriarManager();

            manager.AdicinarColuna(CriarItem(), _ => { }, (_, __) => { });

            Assert.Single(_cards);
            Assert.Single(_posicoes);
        }

        [Fact]
        public void AdicionarColuna_NaoAdiciona_QuandoNull()
        {
            var manager = CriarManager();

            manager.AdicinarColuna(null, _ => { }, (_, __) => { });

            Assert.Empty(_cards);
        }

        [Fact]
        public async Task CarregarSchema_DevePopularLista()
        {
            var manager = CriarManager();

            _dialogMock.Setup(x => x.GetCaminhoArquivo()).Returns("fake.json");

            _converterMock.Setup(x => x.CarregarJsonAsync<Dictionary<int, SchemaDTO>>(It.IsAny<string>())).ReturnsAsync(new Dictionary<int, SchemaDTO>
                {
                    { 1, new SchemaDTO("col1","target","float",false) { NomeModelo = "modelo"} }
                });

            await manager.CarregarSchema(_dialogMock.Object, _converterMock.Object);

            Assert.Single(_cards);
            Assert.Single(_posicoes);
        }

        [Fact]
        public async Task CarregarSchema_NaoFazNada_QuandoCaminhoVazio()
        {
            var manager = CriarManager();

            _dialogMock.Setup(x => x.GetCaminhoArquivo()).Returns("");

            await manager.CarregarSchema(_dialogMock.Object, _converterMock.Object);

            Assert.Empty(_cards);
        }

        [Fact]
        public async Task PreparaParaJson_DeveChamarConversao()
        {
            var manager = CriarManager();

            _cards.Add(CriarCard());

            await manager.PreparaParaJson(_converterMock.Object, "modelo");

            _converterMock.Verify(x =>
                x.ConverteJsonAsync(It.IsAny<Dictionary<int, SchemaDTO>>()),
                Times.Once);
        }
    }
}
