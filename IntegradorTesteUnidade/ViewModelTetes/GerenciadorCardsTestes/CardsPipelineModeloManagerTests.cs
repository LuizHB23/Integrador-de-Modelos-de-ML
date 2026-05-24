using IntegradorAplicacao.DTO;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.Shared.Factory;
using IntegradorViewModel.Shared.Interfaces;
using IntegradorViewModel.Shared.Manager.GerenciadorCards;
using System.Collections.ObjectModel;
using Moq;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;

namespace IntegradorTesteUnidade.ViewModelTetes.GerenciadorCardsTestes
{
    public class CardsPipelineModeloManagerTests
    {
        private readonly Mock<IDialogService> _dialogMock = new();
        private readonly Mock<IConversorJson> _converterMock = new();

        private CardsConfigurarFuncaoManager<FuncaoDTO> CriarManager(
            out ObservableCollection<ConfiguracaoCardFuncaoViewModel> cards,
            out ObservableCollection<int> posicoes)
        {
            cards = new ObservableCollection<ConfiguracaoCardFuncaoViewModel>();
            posicoes = new ObservableCollection<int>();

            return new CardsConfigurarFuncaoManager<FuncaoDTO>(cards, posicoes);
        }

        // =========================
        // ADICIONAR
        // =========================

        [Fact]
        public void AdicionarColuna_NaoFazNada_QuandoItemNull()
        {
            //Arrange
            var manager = CriarManager(out var cards, out var posicoes);

            //Act
            manager.AdicionarCard(null, _ => Task.CompletedTask, (_, __) => { }, _ => Task.CompletedTask);

            //Assert
            Assert.Empty(cards);
            Assert.Empty(posicoes);
        }

        [Fact]
        public void AdicionarColuna_AdicionaCardEPosicao()
        {
            //Arrange
            var manager = CriarManager(out var cards, out var posicoes);

            var item = new FuncaoItemViewModel(1, "Metodo", new List<string> { "linha" });

            //Act
            manager.AdicionarCard(item, _ => Task.CompletedTask, (_, __) => { }, _ => Task.CompletedTask);

            //Assert
            Assert.Single(cards);
            Assert.Single(posicoes);
            Assert.Equal(1, posicoes[0]);
        }

        // =========================
        // CARREGAR
        // =========================

        [Fact]
        public async Task CarregarSchema_JogaExecao_QuandoCaminhoVazio()
        {
            //Arrange
            var manager = CriarManager(out var cards, out var posicoes);

            _dialogMock.Setup(x => x.GetCaminhoArquivo()).Returns("");

            //Act +Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await manager.CarregarPipeline(_converterMock.Object, _ => Task.CompletedTask , _ => Task.CompletedTask, ""));
            Assert.Empty(cards);
            Assert.Empty(posicoes);
        }

        [Fact]
        public async Task CarregarSchema_CarregaCardsCorretamente()
        {
            //Arrange
            var manager = CriarManager(out var cards, out var posicoes);

            _dialogMock.Setup(x => x.GetCaminhoArquivo()).Returns("caminho.json");

            _converterMock.Setup(x => x.CarregarJsonAsync<Dictionary<int, FuncaoDTO>>(It.IsAny<string>()))
                .ReturnsAsync(new Dictionary<int, FuncaoDTO>
                {
                    { 1, new FuncaoDTO(){ NomeFuncao = "Metodo1", Codigo = new List<string>{ "a" }, NomeModelo = "modelo" } },
                    { 2, new FuncaoDTO(){ NomeFuncao = "Metodo2", Codigo = new List<string>{ "b" }, NomeModelo = "modelo" }  }
                });

            //Act
            await manager.CarregarPipeline(_converterMock.Object, _ => Task.CompletedTask, _ => Task.CompletedTask, "");

            //Assert
            Assert.Equal(2, cards.Count);
            Assert.Equal(2, posicoes.Count);
            Assert.Equal(1, posicoes[0]);
            Assert.Equal(2, posicoes[1]);
        }

        // =========================
        // JSON
        // =========================

        [Fact]
        public void PreparaParaJson_DeveChamarConverterComDadosCorretos()
        {
            //Arrange
            var manager = CriarManager(out var cards, out var posicoes);

            var item1 = new FuncaoItemViewModel(1, "Metodo1", new List<string> { "a" });
            var item2 = new FuncaoItemViewModel(2, "Metodo2", new List<string> { "b" });

            manager.AdicionarCard(item1, _ => Task.CompletedTask, (_, __) => { }, _ => Task.CompletedTask);
            manager.AdicionarCard(item2, _ => Task.CompletedTask, (_, __) => { }, _ => Task.CompletedTask);

            //Act
            manager.PreparaParaJson<FuncaoDTOFactory>(_converterMock.Object, "modelo");

            //Assert
            _converterMock.Verify(x =>
                x.ConverteJsonAsync(It.Is<Dictionary<int, FuncaoDTO>>(d =>
                    d.Count == 2 &&
                    d[1].NomeFuncao == "Metodo1" &&
                    d[2].NomeFuncao == "Metodo2"
                )),
                Times.Once);
        }

        // =========================
        // REMOÇÃO (herdado)
        // =========================

        [Fact]
        public void RemoverColuna_RemoveCard()
        {
            //Arrange
            var manager = CriarManager(out var cards, out var posicoes);

            var item = new FuncaoItemViewModel(1, "Metodo", new List<string>());

            manager.AdicionarCard(item, _ => Task.CompletedTask, (_, __) => { }, _ => Task.CompletedTask);

            var card = cards[0];

            //Act
            manager.RemoverCard(card);

            //Assert
            Assert.Empty(cards);
        }

        // =========================
        // ORGANIZAÇÃO (herdado)
        // =========================

        [Fact]
        public void OrganizaPosicao_ReordenaCorretamente()
        {
            //Arrange
            var manager = CriarManager(out var cards, out var posicoes);

            manager.AdicionarCard(new FuncaoItemViewModel(1, "A", new()), _ => Task.CompletedTask, (_, __) => { }, _ => Task.CompletedTask);
            manager.AdicionarCard(new FuncaoItemViewModel(2, "B", new()), _ => Task.CompletedTask, (_, __) => { }, _ => Task.CompletedTask);

            var primeiro = cards[0];

            //Act
            manager.OrganizaPosicao(primeiro, 1);

            //Assert
            Assert.Equal(primeiro, cards[1]);
        }
    }
}
