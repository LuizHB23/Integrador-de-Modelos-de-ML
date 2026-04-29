using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
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
        private readonly Mock<IConverteJson<Dictionary<int, TransformadorDTO>>> _converterMock = new();

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
            var manager = CriarManager(out var cards, out var posicoes);

            manager.AdicinarColuna(null, _ => { }, (_, __) => { });

            Assert.Empty(cards);
            Assert.Empty(posicoes);
        }

        [Fact]
        public void AdicionarColuna_AdicionaCardEPosicao()
        {
            var manager = CriarManager(out var cards, out var posicoes);

            var item = new TransformadorItemViewModel(1, "T1", "caminho");

            manager.AdicinarColuna(item, _ => { }, (_, __) => { });

            Assert.Single(cards);
            Assert.Single(posicoes);
            Assert.Equal(1, posicoes[0]);
        }

        // =========================
        // Carregar
        // =========================

        [Fact]
        public void CarregarTransformador_NaoFazNada_QuandoCaminhoVazio()
        {
            var manager = CriarManager(out var cards, out var posicoes);

            _dialogMock.Setup(x => x.GetCaminhoArquivo()).Returns("");

            manager.CarregarTransformador(_dialogMock.Object, _converterMock.Object);

            Assert.Empty(cards);
            Assert.Empty(posicoes);
        }

        [Fact]
        public void CarregarTransformador_CarregaCardsCorretamente()
        {
            var manager = CriarManager(out var cards, out var posicoes);

            _dialogMock.Setup(x => x.GetCaminhoArquivo()).Returns("caminho.json");

            _converterMock.Setup(x => x.CarregarJson(It.IsAny<string>()))
                .Returns(new Dictionary<int, TransformadorDTO>
                {
                    { 1, new TransformadorDTO("T1", "path1") { NomeModelo = "modelo" } },
                    { 2, new TransformadorDTO("T2", "path2") { NomeModelo = "modelo" } }
                });

            manager.CarregarTransformador(_dialogMock.Object, _converterMock.Object);

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
            var manager = CriarManager(out var cards, out var posicoes);

            var item1 = new TransformadorItemViewModel(1, "T1", "path1");
            var item2 = new TransformadorItemViewModel(2, "T2", "path2");

            manager.AdicinarColuna(item1, _ => { }, (_, __) => { });
            manager.AdicinarColuna(item2, _ => { }, (_, __) => { });

            manager.PreparaParaJson(_converterMock.Object, "modelo");

            _converterMock.Verify(x =>
                x.ConverteJson(It.Is<Dictionary<int, TransformadorDTO>>(d =>
                    d.Count == 2 &&
                    d[1].NomeTransformador == "T1" &&
                    d[2].NomeTransformador == "T2"
                )),
                Times.Once);
        }

        // =========================
        // Remoção herdada
        // =========================

        [Fact]
        public void RemoverColuna_RemoveCard()
        {
            var manager = CriarManager(out var cards, out var posicoes);

            var item = new TransformadorItemViewModel(1, "T1", "path");

            manager.AdicinarColuna(item, _ => { }, (_, __) => { });

            var card = cards[0];

            manager.RemoverCard(card);

            Assert.Empty(cards);
        }

        // =========================
        // Organização herdada
        // =========================

        [Fact]
        public void OrganizaPosicao_ReordenaCorretamente()
        {
            var manager = CriarManager(out var cards, out var posicoes);

            manager.AdicinarColuna(new TransformadorItemViewModel(1, "T1", "p1"), _ => { }, (_, __) => { });
            manager.AdicinarColuna(new TransformadorItemViewModel(2, "T2", "p2"), _ => { }, (_, __) => { });

            var primeiro = cards[0];

            manager.OrganizaPosicao(primeiro, 1);

            Assert.Equal(primeiro, cards[1]);
        }
    }
}
