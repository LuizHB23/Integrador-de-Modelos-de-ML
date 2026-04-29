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
    public class CardsConfigurarSchemaManagerTests
    {
        private readonly ObservableCollection<ConfiguracaoCardSchemaViewModel> _cards = new();
        private readonly ObservableCollection<int> _posicoes = new();

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
        public void CarregarSchema_DevePopularLista()
        {
            var manager = CriarManager();

            var dialogMock = new Mock<IDialogService>();
            var converterMock = new Mock<IConverteJson<Dictionary<int, SchemaDTO>>>();

            dialogMock.Setup(x => x.GetCaminhoArquivo()).Returns("fake.json");

            converterMock.Setup(x => x.CarregarJson(It.IsAny<string>()))
                .Returns(new Dictionary<int, SchemaDTO>
                {
                    { 1, new SchemaDTO("col1","target","float",false) { NomeModelo = "modelo"} }
                });

            manager.CarregarSchema(dialogMock.Object, converterMock.Object);

            Assert.Single(_cards);
            Assert.Single(_posicoes);
        }

        [Fact]
        public void CarregarSchema_NaoFazNada_QuandoCaminhoVazio()
        {
            var manager = CriarManager();

            var dialogMock = new Mock<IDialogService>();
            var converterMock = new Mock<IConverteJson<Dictionary<int, SchemaDTO>>>();

            dialogMock.Setup(x => x.GetCaminhoArquivo()).Returns("");

            manager.CarregarSchema(dialogMock.Object, converterMock.Object);

            Assert.Empty(_cards);
        }

        [Fact]
        public void PreparaParaJson_DeveChamarConversao()
        {
            var manager = CriarManager();
            var converterMock = new Mock<IConverteJson<Dictionary<int, SchemaDTO>>>();

            _cards.Add(CriarCard());

            manager.PreparaParaJson(converterMock.Object, "modelo");

            converterMock.Verify(x =>
                x.ConverteJson(It.IsAny<Dictionary<int, SchemaDTO>>()),
                Times.Once);
        }
    }
}
