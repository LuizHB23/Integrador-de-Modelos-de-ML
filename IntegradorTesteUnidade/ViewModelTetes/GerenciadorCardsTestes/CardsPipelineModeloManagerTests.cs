using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorViewModel.ControleUsuario;
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
    public class CardsPipelineModeloManagerTests
    {
        private readonly Mock<IDialogService> _dialogMock = new();
        private readonly Mock<IConverteJson<Dictionary<int, FuncaoDTO>>> _converterMock = new();

        private CardsPipelineModeloManager CriarManager(
            out ObservableCollection<ConfiguracaoCardFuncaoViewModel> cards,
            out ObservableCollection<int> posicoes)
        {
            cards = new ObservableCollection<ConfiguracaoCardFuncaoViewModel>();
            posicoes = new ObservableCollection<int>();

            return new CardsPipelineModeloManager(cards, posicoes);
        }

        // =========================
        // ADICIONAR
        // =========================

        [Fact]
        public void AdicionarColuna_NaoFazNada_QuandoItemNull()
        {
            var manager = CriarManager(out var cards, out var posicoes);

            manager.AdicinarColuna(null, _ => { }, (_, __) => { }, _ => { });

            Assert.Empty(cards);
            Assert.Empty(posicoes);
        }

        [Fact]
        public void AdicionarColuna_AdicionaCardEPosicao()
        {
            var manager = CriarManager(out var cards, out var posicoes);

            var item = new FuncaoItemViewModel(1, "Metodo", new List<string> { "linha" });

            manager.AdicinarColuna(item, _ => { }, (_, __) => { }, _ => { });

            Assert.Single(cards);
            Assert.Single(posicoes);
            Assert.Equal(1, posicoes[0]);
        }

        // =========================
        // CARREGAR
        // =========================

        [Fact]
        public void CarregarSchema_NaoFazNada_QuandoCaminhoVazio()
        {
            var manager = CriarManager(out var cards, out var posicoes);

            _dialogMock.Setup(x => x.GetCaminhoArquivo()).Returns("");

            manager.CarregarSchema(_dialogMock.Object, _converterMock.Object, _ => { });

            Assert.Empty(cards);
            Assert.Empty(posicoes);
        }

        [Fact]
        public void CarregarSchema_CarregaCardsCorretamente()
        {
            var manager = CriarManager(out var cards, out var posicoes);

            _dialogMock.Setup(x => x.GetCaminhoArquivo()).Returns("caminho.json");

            _converterMock.Setup(x => x.CarregarJson(It.IsAny<string>()))
                .Returns(new Dictionary<int, FuncaoDTO>
                {
                    { 1, new FuncaoDTO("Metodo1", new List<string>{ "a" }, "modelo") },
                    { 2, new FuncaoDTO("Metodo2", new List<string>{ "b" }, "modelo") }
                });

            manager.CarregarSchema(_dialogMock.Object, _converterMock.Object, _ => { });

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

            var item1 = new FuncaoItemViewModel(1, "Metodo1", new List<string> { "a" });
            var item2 = new FuncaoItemViewModel(2, "Metodo2", new List<string> { "b" });

            manager.AdicinarColuna(item1, _ => { }, (_, __) => { }, _ => { });
            manager.AdicinarColuna(item2, _ => { }, (_, __) => { }, _ => { });

            manager.PreparaParaJson(_converterMock.Object, "modelo");

            _converterMock.Verify(x =>
                x.ConverteJson(It.Is<Dictionary<int, FuncaoDTO>>(d =>
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
            var manager = CriarManager(out var cards, out var posicoes);

            var item = new FuncaoItemViewModel(1, "Metodo", new List<string>());

            manager.AdicinarColuna(item, _ => { }, (_, __) => { }, _ => { });

            var card = cards[0];

            manager.RemoverColuna(card);

            Assert.Empty(cards);
        }

        // =========================
        // ORGANIZAÇÃO (herdado)
        // =========================

        [Fact]
        public void OrganizaPosicao_ReordenaCorretamente()
        {
            var manager = CriarManager(out var cards, out var posicoes);

            manager.AdicinarColuna(new FuncaoItemViewModel(1, "A", new()), _ => { }, (_, __) => { }, _ => { });
            manager.AdicinarColuna(new FuncaoItemViewModel(2, "B", new()), _ => { }, (_, __) => { }, _ => { });

            var primeiro = cards[0];

            manager.OrganizaPosicao(primeiro, 1);

            Assert.Equal(primeiro, cards[1]);
        }
    }
}
