using InetradorAplicacao.DTO;
using InetradorAplicacao.Gerenciador;
using IntegradorAplicacao.ConversorJson;
using IntegradorViewModel.Interfaces;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.InserirModelo;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.ViewModelTetes.PagesTestes.InserirModeloTestes.InserirModeloTestes
{
    public class InserirModeloViewModeloTests
    {
        private readonly Mock<IGerenciador<ModeloDTO>> _mockGerenciador;
        private readonly Mock<IConverteJson<ModeloDTO>> _mockConversor;
        private readonly Mock<IDialogService> _mockDialog;
        private readonly NavigationService _navigation;

        public InserirModeloViewModeloTests()
        {
            var mockProvider = new Mock<IServiceProvider>();
            _navigation = new NavigationService(mockProvider.Object);
            _mockConversor = new Mock<IConverteJson<ModeloDTO>>();
            _mockGerenciador = new Mock<IGerenciador<ModeloDTO>>();
            _mockDialog = new Mock<IDialogService>();
        }

        [Fact]
        public void RetornaOnnxQuandoArquivoForValidoEmBuscaModeloCommand()
        {
            //Arrange
            string caminho = "C:/FakePath/modelo.onnx";
            _mockDialog.Setup(f => f.GetCaminhoArquivo()).Returns(caminho);

            var viewModel = new InserirModeloViewModel(_navigation, _mockGerenciador.Object, _mockDialog.Object, _mockConversor.Object);

            //Act
            viewModel.BuscaModeloCommand.Execute(null);

            //Assert
            Assert.Equal(".onnx", Path.GetExtension(viewModel.NomeCaminho));
            Assert.Equal(caminho, viewModel.CaminhoModelo);
        }

        [Theory]
        [InlineData ("C:/FakePath/modelo.qualquer")]
        [InlineData("")]
        [InlineData("SimulandoCancelar")]
        [InlineData(null)]
        public void RetornaNadaQuandoArquivoNaoForOnnxEmBuscaModeloCommand(string? caminho)
        {
            //Arrange
            _mockDialog.Setup(f => f.GetCaminhoArquivo()).Returns(caminho);

            var viewModel = new InserirModeloViewModel(_navigation, _mockGerenciador.Object, _mockDialog.Object, _mockConversor.Object);

            //Act
            viewModel.BuscaModeloCommand.Execute(null);
            string fakeCaminho = viewModel.CaminhoModelo;
            string fakeOnnx = viewModel.NomeModelo;


            //Assert
            Assert.Empty(fakeCaminho);
            Assert.Empty(fakeOnnx);
        }

        [Fact]
        public void RetornaOkParaCaminhoModeloModificadoEmNavigateToConfigurarSchemaCommand()
        {
            //Arrange
            _mockDialog.Setup(f => f.GetCaminhoArquivo()).Returns("C://FakePath//modelo.onnx");
            _mockGerenciador.Setup(f => f.Salvar(It.IsAny<ModeloDTO>())).Returns("C://FakePathMudado//modelo.onnx");

            var viewModel = new InserirModeloViewModel(_navigation, _mockGerenciador.Object, _mockDialog.Object, _mockConversor.Object);
            viewModel.BuscaModeloCommand.Execute(null);

            //Act
            viewModel.NavigateToConfigurarSchemaCommand.Execute(null);

            //Assert
            Assert.Equal("C://FakePathMudado//modelo.onnx", viewModel.CaminhoModelo);
        }


        [Fact]
        public void RetornaOkParaGerenciadorSalvarVisitadoEmNavigateToConfigurarSchemaCommand()
        {
            //Arrange
            _mockGerenciador.Setup(f => f.Salvar(It.IsAny<ModeloDTO>())).Returns("C://FakePath//modelo.onnx");

            var viewModel = new InserirModeloViewModel(_navigation, _mockGerenciador.Object, _mockDialog.Object, _mockConversor.Object);

            //Act
            viewModel.NavigateToConfigurarSchemaCommand.Execute(null);

            //Assert
            _mockGerenciador.Verify(f => f.Salvar(It.IsAny<ModeloDTO>()), Times.Once);
        }

        [Fact]
        public void RetornaOkParaConversorConverteJsonVisitadoEmNavigateToConfigurarSchemaCommand()
        {
            //Arrange
            _mockGerenciador.Setup(f => f.Salvar(It.IsAny<ModeloDTO>())).Returns("C://FakePath//modelo.onnx");

            var viewModel = new InserirModeloViewModel(_navigation, _mockGerenciador.Object, _mockDialog.Object, _mockConversor.Object);

            //Act
            viewModel.NavigateToConfigurarSchemaCommand.Execute(null);

            //Assert
            _mockConversor.Verify(f => f.ConverteJson(It.IsAny<ModeloDTO>()), Times.Once);
        }

        [Theory]
        [InlineData("     ", "Tipo Qualquer", "Caminho Qualquer")]
        [InlineData("Nome Qualquer", "Tipo Qualquer", "")]
        [InlineData("Nome Qualquer", "", "Caminho Qualquer")]
        [InlineData("", "Tipo Qualquer", "Caminho Qualquer")]
        [InlineData("Nome Qualquer", "", "")]
        [InlineData("r", "Tipo Qualquer", "")]
        [InlineData("", "", "Caminho Qualquer")]
        [InlineData("", "", "")]
        [InlineData("   ", "", "")]
        public void RertonaNaoOkParaPartesNaoPreenchidasEmNavigateToConfigurarSchemaCommand(string nomeModelo, string tipoModelo, string caminhoModelo)
        {
            //Arrange
            var viewModel = new InserirModeloViewModel(_navigation, _mockGerenciador.Object, _mockDialog.Object, _mockConversor.Object);

            viewModel.NomeModelo = nomeModelo;
            viewModel.TipoModelo = tipoModelo;
            viewModel.CaminhoModelo = caminhoModelo;

            //Act
            viewModel.NavigateToConfigurarSchemaCommand.Execute(null);

            //Assert
            _mockConversor.Verify(f => f.ConverteJson(It.IsAny<ModeloDTO>()), Times.Never);
        }
    }
}
