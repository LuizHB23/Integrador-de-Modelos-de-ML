using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Gerenciador;
using IntegradorAplicacao.ConversorJson;
using IntegradorViewModel.Interfaces;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.InserirModelo;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.ViewModelTetes.PagesTestes.InserirModeloTestes
{
    public class InserirModeloViewModelTests
    {
        private readonly Mock<IGerenciador<ModeloDTO>> _mockGerenciador;
        private readonly Mock<IConverteJson<ModeloDTO>> _mockConversor;
        private readonly Mock<IDialogService> _mockDialog;
        private readonly Mock<INavigationService> _mockNavigation;
        private readonly InserirModeloViewModel _viewModel;

        public InserirModeloViewModelTests()
        {
            _mockNavigation = new Mock<INavigationService>();
            _mockConversor = new Mock<IConverteJson<ModeloDTO>>();
            _mockGerenciador = new Mock<IGerenciador<ModeloDTO>>();
            _mockDialog = new Mock<IDialogService>();

            _viewModel = new InserirModeloViewModel(_mockNavigation.Object, _mockGerenciador.Object, _mockDialog.Object, _mockConversor.Object);
        }

        [Fact]
        public void RetornaOnnxQuandoArquivoForValidoEmBuscaModeloCommand()
        {
            //Arrange
            string caminho = "C:/FakePath/modelo.onnx";
            _mockDialog.Setup(f => f.GetCaminhoArquivo()).Returns(caminho);

            //Act
            _viewModel.BuscaModeloCommand.Execute(null);

            //Assert
            Assert.Equal(".onnx", Path.GetExtension(_viewModel.NomeCaminho));
            Assert.Equal(caminho, _viewModel.CaminhoModelo);
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

            //Act
            _viewModel.BuscaModeloCommand.Execute(null);
            string fakeCaminho = _viewModel.CaminhoModelo;
            string fakeOnnx = _viewModel.NomeModelo;


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

            _viewModel.BuscaModeloCommand.Execute(null);
            _viewModel.NomeModelo = "Nome Qualquer";
            _viewModel.TipoModelo = "Tipo Qualquer";

            //Act
            _viewModel.NavigateToConfigurarSchemaCommand.Execute(null);

            //Assert
            Assert.Equal("C://FakePathMudado//modelo.onnx", _viewModel.CaminhoModelo);
        }


        [Fact]
        public void RetornaOkParaGerenciadorSalvarVisitadoEmNavigateToConfigurarSchemaCommand()
        {
            //Arrange
            _mockGerenciador.Setup(f => f.Salvar(It.IsAny<ModeloDTO>())).Returns("C://FakePath//modelo.onnx");

            _viewModel.NomeModelo = "Nome Qualquer";
            _viewModel.TipoModelo = "Tipo Qualquer";
            _viewModel.CaminhoModelo = "Caminho Qualquer";

            //Act
            _viewModel.NavigateToConfigurarSchemaCommand.Execute(null);

            //Assert
            _mockGerenciador.Verify(f => f.Salvar(It.IsAny<ModeloDTO>()), Times.Once);
        }

        [Fact]
        public void RetornaOkParaConversorConverteJsonVisitadoEmNavigateToConfigurarSchemaCommand()
        {
            //Arrange
            _mockGerenciador.Setup(f => f.Salvar(It.IsAny<ModeloDTO>())).Returns("C://FakePath//modelo.onnx");

            _viewModel.NomeModelo = "Nome Qualquer";
            _viewModel.TipoModelo = "Tipo Qualquer";
            _viewModel.CaminhoModelo = "Caminho Qualquer";

            //Act
            _viewModel.NavigateToConfigurarSchemaCommand.Execute(null);

            //Assert
            _mockConversor.Verify(f => f.ConverteJson(It.IsAny<ModeloDTO>()), Times.Once);
        }

        [Fact]
        public void RetornaOkParaNavigateToVisitadoEProgramadoParaConfigurarSchemaViewModelEmNavigateToConfigurarSchemaCommand()
        {
            //Arrange
            _viewModel.NomeModelo = "Nome Qualquer";
            _viewModel.TipoModelo = "Tipo Qualquer";
            _viewModel.CaminhoModelo = "Caminho Qualquer";

            //Act
            _viewModel.NavigateToConfigurarSchemaCommand.Execute(null);

            //Assert
            _mockNavigation.Verify(f => f.NavigateTo<ConfigurarSchemaViewModel>(), Times.Once);
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
        public void RetornaMensageBoxParaPartesNaoPreenchidasEmNavigateToConfigurarSchemaCommand(string nomeModelo, string tipoModelo, string caminhoModelo)
        {
            //Arrange
            _viewModel.NomeModelo = nomeModelo;
            _viewModel.TipoModelo = tipoModelo;
            _viewModel.CaminhoModelo = caminhoModelo;

            //Act
            _viewModel.NavigateToConfigurarSchemaCommand.Execute(null);

            //Assert
            _mockConversor.Verify(f => f.ConverteJson(It.IsAny<ModeloDTO>()), Times.Never);
            _mockDialog.Verify(f => f.ShowMessage(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void RetornaMensageBoxParaNomeModeloInvalidoEmNavigateToConfigurarSchemaCommand()
        {
            //Arrange
            _viewModel.NomeModelo = "@#$%¨&*(¨%&*&";
            _viewModel.TipoModelo = "Tipo Qualquer";
            _viewModel.CaminhoModelo = "Caminho Qualquer";

            //Act
            _viewModel.NavigateToConfigurarSchemaCommand.Execute(null);

            //Assert
            _mockDialog.Verify(f => f.ShowMessage(It.IsAny<string>()), Times.Once);
        }
    }
}
