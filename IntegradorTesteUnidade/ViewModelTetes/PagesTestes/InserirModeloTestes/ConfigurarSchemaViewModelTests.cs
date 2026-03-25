using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.InserirModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using Bogus;
using Moq;
using IntegradorViewModel.Shared.Interfaces;
using IntegradorViewModel.Shared.Context;

namespace IntegradorTesteUnidade.ViewModelTetes.PagesTestes.InserirModeloTestes
{
    public class ConfigurarSchemaViewModelTests
    {
        private readonly Mock<IConverteJson<Dictionary<int, SchemaDTO>>> _mockConversor;
        private readonly Mock<IDialogService> _mockDialog;
        private readonly Mock<INavigationService> _mockNavigation;
        private readonly Mock<IContext<ModeloDTO>> _mockContext;
        private readonly Mock<IPathProvider> _mockProvider;

        private readonly ConfigurarSchemaViewModel _viewModel;
        private Faker<SchemaItemViewModel> _fakerSchmeaItem;

        public ConfigurarSchemaViewModelTests()
        {
            _mockConversor = new Mock<IConverteJson<Dictionary<int, SchemaDTO>>>();
            _mockDialog = new Mock<IDialogService>();
            _mockNavigation = new Mock<INavigationService>();
            _mockContext = new Mock<IContext<ModeloDTO>>();
            _mockProvider = new Mock<IPathProvider>();

            _mockContext.Setup(f => f.RecebeMensagem()).Returns(new ModeloDTO("Nome Qualquer", "", ""));

            _viewModel = new ConfigurarSchemaViewModel(_mockNavigation.Object, _mockDialog.Object, _mockConversor.Object, _mockContext.Object, _mockProvider.Object);

            _fakerSchmeaItem = new Faker<SchemaItemViewModel>()
                                .CustomInstantiator(f => new SchemaItemViewModel(
                                    posicao: 0,
                                    nomeColuna: f.Database.Column(),
                                    finalidade: f.PickRandom("Input", "Output", "Target"),
                                    tipo: f.PickRandom("Int32", "Double", "String", "Boolean"),
                                    categorico: f.Random.Bool()
                                ));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RetornaEhIgualParaElementosAdicionadosNaCardsSchemaEmAdicinarColunaCommand(bool categorico)
        {
            //Arrange
            string nomeColuna = "Nome Coluna Qualquer";
            string finalidade = "Finalidade Qualquer";
            string tipo = "Tipo Qualquer";

            _viewModel.NomeColuna = nomeColuna;
            _viewModel.Finalidade = finalidade;
            _viewModel.Tipo = tipo;
            _viewModel.Categorico = categorico;

            //Act
            _viewModel.AdicinarColunaCommand.Execute(null);

            //Assert
            Assert.Single(_viewModel.CardsSchema);
            Assert.Equal(nomeColuna, _viewModel.CardsSchema.Single().NomeColuna);
            Assert.Equal(finalidade, _viewModel.CardsSchema.Single().Finalidade);
            Assert.Equal(tipo, _viewModel.CardsSchema.Single().Tipo);
            Assert.Equal(categorico, _viewModel.CardsSchema.Single().Categorico);

            Assert.Single(_viewModel.OpcoesPosicao);
        }

        [Fact]
        public void RetornaStringsVaziasFalseNaCardsSchemaAoAdicionarElementoEmAdicinarColunaCommand()
        {
            //Arrange
            _viewModel.NomeColuna = "Nome Coluna Qualquer";
            _viewModel.Finalidade = "Finalidade Qualquer";
            _viewModel.Tipo = "Tipo Qualquer";
            _viewModel.Categorico = true;

            //Act
            _viewModel.AdicinarColunaCommand.Execute(null);

            //Assert
            Assert.Empty(_viewModel.NomeColuna);
            Assert.Empty(_viewModel.Finalidade);
            Assert.Empty(_viewModel.Tipo);
            Assert.False(_viewModel.Categorico);
        }

        [Fact]
        public void RetornaMensageBoxParaPartesNaoPreenchidasEmAdicinarColunaCommand()
        {
            //Arrange + Act
            _viewModel.AdicinarColunaCommand.Execute(null);

            //Assert
            _mockDialog.Verify(f => f.ShowMessage($"Preencha corretamente os campos", "Campos Faltantes"), Times.Once);
        }


        [Fact]
        public void RetornaEhIgualParaPosicoesCorretasAoAdicionarMultiplosElementosEmAdicinarColunaCommand()
        {
            // Arrange
            var itensFake = _fakerSchmeaItem.Generate(3);

            Action<ConfiguracaoCardSchemaViewModel> removerFake = (vm) =>
            {
                _viewModel.CardsSchema.Remove(vm);
            };

            // Act
            foreach (var item in itensFake)
            {
                _viewModel.NomeColuna = item.NomeColuna;
                _viewModel.Finalidade = item.Finalidade;
                _viewModel.Tipo = item.Tipo;
                _viewModel.Categorico = item.Categorico;
                _viewModel.AdicinarColunaCommand.Execute(null);
            }

            // Assert
            Assert.Equal(3, _viewModel.CardsSchema.Count);
            Assert.Equal(1, _viewModel.CardsSchema[0].Posicao);
            Assert.Equal(2, _viewModel.CardsSchema[1].Posicao);
            Assert.Equal(3, _viewModel.CardsSchema[2].Posicao);
        }

        [Fact]
        public void RetornaOkParaConversorComDicionarioCorretoAoPrepararParaJsonEmAdicinarColunaCommand()
        {
            // Arrange
            var itensFakes = _fakerSchmeaItem.Generate(3);

            //Act
            foreach (var item in itensFakes)
            {
                _viewModel.NomeColuna = item.NomeColuna;
                _viewModel.Finalidade = item.Finalidade;
                _viewModel.Tipo = item.Tipo;
                _viewModel.Categorico = item.Categorico;
                _viewModel.AdicinarColunaCommand.Execute(null);
            }

            // Assert
            _mockConversor.Verify(f => f.ConverteJson(It.Is<Dictionary<int, SchemaDTO>>(dict =>
                dict.Count == 3 &&
                dict.ContainsKey(1) &&
                dict.ContainsKey(2) &&
                dict.ContainsKey(3) &&
                dict[1].NomeColuna == itensFakes[0].NomeColuna
            )), Times.AtLeastOnce);
        }

        [Fact]
        public void RetornaOkParaServicosVisitadosQuandoCaminhoNaoEhVazioEmCarregarSchemaCommand()
        {
            //Arrange
            string caminho = "Caminho Qualquer";
            _mockDialog.Setup(f => f.GetCaminhoArquivo()).Returns(caminho);
            _mockConversor.Setup(f => f.CarregarJson(caminho)).Returns(new Dictionary<int, SchemaDTO>());

            //Act
            _viewModel.CarregarSchemaCommand.Execute(null);

            //Assert
            _mockDialog.Verify(f => f.GetCaminhoArquivo(), Times.Once);
            _mockConversor.Verify(f => f.CarregarJson("Caminho Qualquer"), Times.Once);
        }

        [Fact]
        public void RetornaOkParaNaoFazerNadaQuandoCaminhoVazioEmCarregarSchemaCommand()
        {
            _mockDialog.Setup(f => f.GetCaminhoArquivo()).Returns("");

            _viewModel.CarregarSchemaCommand.Execute(null);

            _mockConversor.Verify(f => f.CarregarJson(It.IsAny<string>()), Times.Never);
            Assert.Empty(_viewModel.CardsSchema);
        }

        [Fact]
        public void RetornaEhIgualParaJsonCarregadoComSucessoEmCarregarSchemaCommand()
        {
            //Arrange
            string caminho = "Caminho Qualquer";
            _mockDialog.Setup(f => f.GetCaminhoArquivo()).Returns(caminho);

            string nomeColuna = "Nome Coluna Qualquer";
            string finalidade = "Finalidade Qualquer";
            string tipo = "Tipo Qualquer";
            bool categorico = true;

            var schema = new Dictionary<int, SchemaDTO>();
            schema.Add(1, new SchemaDTO(nomeColuna, finalidade, tipo, categorico, "Modelo Qualquer"));
            _mockConversor.Setup(f => f.CarregarJson(caminho)).Returns(schema);

            //Act
            _viewModel.CarregarSchemaCommand.Execute(null);

            //Assert
            Assert.Single(_viewModel.CardsSchema);
            Assert.Equal(nomeColuna, _viewModel.CardsSchema.Single().NomeColuna);
            Assert.Equal(finalidade, _viewModel.CardsSchema.Single().Finalidade);
            Assert.Equal(tipo, _viewModel.CardsSchema.Single().Tipo);
            Assert.True(_viewModel.CardsSchema.Single().Categorico);

            Assert.Single(_viewModel.OpcoesPosicao);
            Assert.Equal(1, _viewModel.OpcoesPosicao.Single());
        }

        [Fact]
        public void RetornaVazioQuandoElementoRemovidoDeCardsSchemaEmRemoverColuna()
        {
            //Arrange
            Action<ConfiguracaoCardSchemaViewModel> removerFake = (vm) =>
            {
                _viewModel.CardsSchema.Remove(vm);
            };

            ConfiguracaoCardSchemaViewModel? recebidoVm = null;
            int recebidoIndice = -1;
            Action<ConfiguracaoCardSchemaViewModel, int> trocarFake = (vm, indice) =>
            {
                recebidoVm = vm;
                recebidoIndice = indice;
            };

            var schemaItem = new SchemaItemViewModel(1, "Nome Qualquer", "Finalidade Qualquer", "Tipo Qualquer", false);

            var cardSchema = new ConfiguracaoCardSchemaViewModel(schemaItem, removerFake, trocarFake);

            _viewModel.CardsSchema.Add(cardSchema);

            //Act
            cardSchema.FoiRemovidoCommand.Execute(null);

            //Assert
            Assert.Empty(_viewModel.CardsSchema);
        }

        [Fact]
        public void RetornaEhIgualParaNovaPosicaoAposReposicionadoEmOrganizaPosicao()
        {
            // Arrange
            var itensFakes = _fakerSchmeaItem.Generate(6);
            foreach (var item in itensFakes)
            {
                _viewModel.NomeColuna = item.NomeColuna;
                _viewModel.Finalidade = item.Finalidade;
                _viewModel.Tipo = item.Tipo;
                _viewModel.Categorico = item.Categorico;
                _viewModel.AdicinarColunaCommand.Execute(null);
            }

            int posicaoOriginal = Random.Shared.Next(0, 6);
            var cardQualquer = _viewModel.CardsSchema[posicaoOriginal];
            int posicaoNova = Random.Shared.Next(0, 6);

            if (posicaoNova == posicaoOriginal)
            {
                if(posicaoNova < 5)
                {
                    posicaoNova++;
                }
                else
                {
                    posicaoNova--;
                }
            }

            //Act
            cardQualquer.Posicao = posicaoNova + 1;

            //Assert
            Assert.Equal(posicaoNova + 1, cardQualquer.Posicao);
            Assert.Equal(cardQualquer, _viewModel.CardsSchema[posicaoNova]);
            Assert.NotEqual(posicaoOriginal + 1, cardQualquer.Posicao);
        }

        [Fact]
        public void RetornaOkParaCardSchemaNaoVazioEmNavigateToCarregarDadosCommand()
        {
            //Arrange
            _viewModel.NomeColuna = "Nome Coluna Qualquer";
            _viewModel.Finalidade = "Finalidade Qualquer";
            _viewModel.Tipo = "Tipo Qualquer";
            _viewModel.Categorico = true;
            _viewModel.AdicinarColunaCommand.Execute(null);

            //Act
            _viewModel.NavigateToCarregarDadosCommand.Execute(null);

            //Assert
            _mockNavigation.Verify(f => f.NavigateTo<CarregarDadosViewModel>(), Times.Once);
            _mockDialog.Verify(f => f.ShowMessage("Não se pode criar um Schema vazio.", "Schema Vazio"), Times.Never);
        }

        [Fact]
        public void RetornaMensageBoxParaCardSchemaVazioEmNavigateToCarregarDadosCommand()
        {
            //Arrange + Act
            _viewModel.NavigateToCarregarDadosCommand.Execute(null);

            //Assert
            _mockDialog.Verify(f => f.ShowMessage("Não se pode criar um Schema vazio.", "Schema Vazio"), Times.Once);
            _mockNavigation.Verify(f => f.NavigateTo<CarregarDadosViewModel>(), Times.Never);
        }

        [Fact]
        public void RetornaOkParaFluxoScopedEncerradoERetonoHomeEmNavigateToHomeCommand()
        {
            //Arrange + Act
            _viewModel.NavigateToHomeCommand.Execute(null);

            //Assert
            _mockNavigation.Verify(f => f.EndFlow(), Times.Once);
            _mockNavigation.Verify(f => f.NavigateTo<HomeViewModel>(), Times.Once);
        }
    }
}
