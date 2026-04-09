using IntegradorViewModel.ControleUsuario.ConfiguracaoCard;
using IntegradorViewModel.Shared.Manager.GerenciadorCards;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace IntegradorTesteUnidade.ViewModelTetes.GerenciadorCardsTestes
{
    public class CardsManagerTests
    {
        private class CardFake : IConfiguracaoCard
        {
            public bool EstouReposicionando { get; set; }
            public ObservableCollection<int> OpcoesPosicao { get; set; }
            public int Posicao { get; set; }
        }

        private class TestManager : CardsManager<CardFake>
        {
            public TestManager(ObservableCollection<CardFake> cards, ObservableCollection<int> posicoes)
                : base(cards, posicoes) { }
        }

        // =========================
        // HELPERS
        // =========================
        private TestManager CriarManagerCom3Itens(out ObservableCollection<CardFake> lista)
        {
            lista = new ObservableCollection<CardFake>
            {
                new CardFake(),
                new CardFake(),
                new CardFake()
            };

            var posicoes = new ObservableCollection<int> { 1, 2, 3 };

            return new TestManager(lista, posicoes);
        }

        // =========================
        // TESTES
        // =========================

        [Fact]
        public void AtualizaPosicoes_DeveAjustarSequenciaCorretamente()
        {
            var manager = CriarManagerCom3Itens(out var lista);

            manager.AtualizaPosicoes();

            Assert.Equal(1, lista[0].Posicao);
            Assert.Equal(2, lista[1].Posicao);
            Assert.Equal(3, lista[2].Posicao);
        }

        [Fact]
        public void AtualizaPosicoes_DeveAtribuirMesmaListaDeOpcoes()
        {
            var manager = CriarManagerCom3Itens(out var lista);

            manager.AtualizaPosicoes();

            var opcoes = lista[0].OpcoesPosicao;

            Assert.Same(opcoes, lista[1].OpcoesPosicao);
            Assert.Same(opcoes, lista[2].OpcoesPosicao);
        }

        [Fact]
        public void OrganizaPosicao_DeveMoverItemParaNovaPosicao()
        {
            var manager = CriarManagerCom3Itens(out var lista);

            var item = lista[0];

            manager.OrganizaPosicao(item, 2);

            Assert.Equal(item, lista[2]);
        }

        [Fact]
        public void OrganizaPosicao_DeveReordenarPosicoesAposMover()
        {
            var manager = CriarManagerCom3Itens(out var lista);

            var item = lista[0];

            manager.OrganizaPosicao(item, 2);

            Assert.Equal(1, lista[0].Posicao);
            Assert.Equal(2, lista[1].Posicao);
            Assert.Equal(3, lista[2].Posicao);
        }

        [Fact]
        public void RemoverColuna_DeveRemoverItemDaLista()
        {
            var manager = CriarManagerCom3Itens(out var lista);

            var item = lista[1];

            manager.RemoverColuna(item);

            Assert.DoesNotContain(item, lista);
            Assert.Equal(2, lista.Count);
        }

        [Fact]
        public void RemoverColuna_DeveAtualizarPosicoesAposRemover()
        {
            var manager = CriarManagerCom3Itens(out var lista);

            manager.RemoverColuna(lista[0]);

            Assert.Equal(1, lista[0].Posicao);
            Assert.Equal(2, lista[1].Posicao);
        }

        [Fact]
        public void RemoverColuna_DeveRemoverUltimaPosicaoDaListaDePosicoes()
        {
            var lista = new ObservableCollection<CardFake>
            {
                new CardFake(),
                new CardFake(),
                new CardFake()
            };

            var posicoes = new ObservableCollection<int> { 1, 2, 3 };

            var manager = new TestManager(lista, posicoes);

            manager.RemoverColuna(lista[0]);

            Assert.DoesNotContain(3, posicoes);
        }

        [Fact]
        public void AtualizaPosicoes_DeveDesligarFlagEstouReposicionandoNoFinal()
        {
            var manager = CriarManagerCom3Itens(out var lista);

            manager.AtualizaPosicoes();

            Assert.All(lista, item => Assert.False(item.EstouReposicionando));
        }
    }
}
