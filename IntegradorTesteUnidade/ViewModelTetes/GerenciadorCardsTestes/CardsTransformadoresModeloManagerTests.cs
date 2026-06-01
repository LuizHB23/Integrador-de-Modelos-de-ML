using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorViewModel.ControleUsuario.ConfiguracaoCard;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.Shared.Interfaces;
using IntegradorViewModel.Shared.Manager.GerenciadorCards;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace IntegradorTesteUnidade.ViewModelTetes.GerenciadorCardsTestes
{
    public class CardsTransformadoresModeloManagerTests
    {
        private readonly Mock<IDialogService> _dialogMock = new();
        private readonly Mock<IConversorJson> _converterMock = new();

        private CardsTransformadoresModeloManager CriarManager(
            out ObservableCollection<ConfiguracaoCardTransformadorViewModel> cards,
            out ObservableCollection<int> posicoes)
        {
            cards = new ObservableCollection<ConfiguracaoCardTransformadorViewModel>();
            posicoes = new ObservableCollection<int>();

            return new CardsTransformadoresModeloManager(cards, posicoes);
        }

        // =========================
        // Adicionar
        // =========================

        [Fact]
        public void AdicionarColuna_NaoFazNada_QuandoItemNull()
        {
            //Arrange
            var manager = CriarManager(out var cards, out var posicoes);

            //Act
            manager.AdicinarColuna(null, _ => Task.CompletedTask, (_, __) => Task.CompletedTask);

            //Assert
            Assert.Empty(cards);
            Assert.Empty(posicoes);
        }

        [Fact]
        public void AdicionarColuna_AdicionaCardEPosicao()
        {
            //Arrange
            var manager = CriarManager(out var cards, out var posicoes);

            var item = new TransformadorItemViewModel(1, "T1", "caminho");

            //Act
            manager.AdicinarColuna(item, _ => Task.CompletedTask, (_, __) => Task.CompletedTask);

            //Assert
            Assert.Single(cards);
            Assert.Single(posicoes);
            Assert.Equal(1, posicoes[0]);
        }

        // =========================
        // Carregar
        // =========================

        [Fact]
        public async Task CarregarTransformador_NaoFazNada_QuandoCaminhoVazio()
        {
            //Arrange
            var manager = CriarManager(out var cards, out var posicoes);

            _dialogMock.Setup(x => x.GetCaminhoArquivo()).Returns("");

            //Act
            await manager.CarregarTransformador(_dialogMock.Object, _converterMock.Object);

            //Assert
            Assert.Empty(cards);
            Assert.Empty(posicoes);
        }

        [Fact]
        public async Task CarregarTransformador_CarregaCardsCorretamente()
        {
            //Arrange
            var manager = CriarManager(out var cards, out var posicoes);

            _dialogMock.Setup(x => x.GetCaminhoArquivo()).Returns("caminho.json");

            _converterMock.Setup(x => x.CarregarJsonAsync<Dictionary<int, TransformadorDTO>>(It.IsAny<string>())).ReturnsAsync(new Dictionary<int, TransformadorDTO>
                {
                    { 1, new TransformadorDTO("T1", "path1") { NomeModelo = "modelo" } },
                    { 2, new TransformadorDTO("T2", "path2") { NomeModelo = "modelo" } }
                });

            //Act
            await manager.CarregarTransformador(_dialogMock.Object, _converterMock.Object);

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
        public async Task PreparaParaJson_DeveChamarConverterComDadosCorretos()
        {
            //Arrange
            string nomeModelo = "modelo";
            var manager = CriarManager(out var cards, out var posicoes);

            var item1 = new TransformadorItemViewModel(1, "T1", "path1");
            var item2 = new TransformadorItemViewModel(2, "T2", "path2");

            manager.AdicinarColuna(item1, _ => Task.CompletedTask, (_, __) => Task.CompletedTask);
            manager.AdicinarColuna(item2, _ => Task.CompletedTask, (_, __) => Task.CompletedTask);

            //Act
            await manager.PreparaParaJson(_converterMock.Object, nomeModelo);

            //Assert
            _converterMock.Verify(x =>
                x.ConverteJsonAsync(It.Is<Dictionary<int, TransformadorDTO>>(d =>
                    d.Count == 2 &&
                    d[1].NomeTransformador == "T1" &&
                    d[2].NomeTransformador == "T2"
                ), nomeModelo),
                Times.Once);
        }

        // =========================
        // Remoção herdada
        // =========================

        [Fact]
        public async Task RemoverColuna_RemoveCard()
        {
            //Arrange
            var manager = CriarManager(out var cards, out var posicoes);

            var item = new TransformadorItemViewModel(1, "T1", "path");

            manager.AdicinarColuna(item, _ => Task.CompletedTask, (_, __) => Task.CompletedTask);

            var card = cards[0];

            //Act
            await manager.RemoverCard(card);

            //Assert
            Assert.Empty(cards);
        }

        // =========================
        // Organização herdada
        // =========================

        [Fact]
        public async Task OrganizaPosicao_ReordenaCorretamente()
        {
            //Arrange
            var manager = CriarManager(out var cards, out var posicoes);

            manager.AdicinarColuna(new TransformadorItemViewModel(1, "T1", "p1"), _ => Task.CompletedTask, (_, __) => Task.CompletedTask);
            manager.AdicinarColuna(new TransformadorItemViewModel(2, "T2", "p2"), _ => Task.CompletedTask, (_, __) => Task.CompletedTask);

            var primeiro = cards[0];

            //Act
            await manager.OrganizaPosicao(primeiro, 1);

            //Assert
            Assert.Equal(primeiro, cards[1]);
        }
    }
}
