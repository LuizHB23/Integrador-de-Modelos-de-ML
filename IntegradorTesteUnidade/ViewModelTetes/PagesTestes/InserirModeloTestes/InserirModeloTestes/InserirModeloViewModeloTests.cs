using InetradorAplicacao.DTO;
using InetradorAplicacao.Gerenciador;
using IntegradorAplicacao.ConversorJSON;
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
        private readonly Mock<IConverteJSON<ModeloDTO>> _mockConversor;
        private readonly NavigationService _navigation;

        public InserirModeloViewModeloTests()
        {
            var mockProvider = new Mock<IServiceProvider>();
            _navigation = new NavigationService(mockProvider.Object);
            _mockConversor = new Mock<IConverteJSON<ModeloDTO>>();

        }

        [Fact]
        public void RetornaONNXQuandoArquivoForValidoParaBuscaModeloCommand()
        {
            //Arrange
            var mockGerenciador = new Mock<IGerenciador<ModeloDTO>>();
            var mockDialog = new Mock<IDialogService>();

            string caminho = "C:/FakePath/modelo.onnx";
            mockDialog.Setup(f => f.GetCaminhoArquivo()).Returns(caminho);

            var viewModel = new InserirModeloViewModel(_navigation, mockGerenciador.Object, mockDialog.Object, _mockConversor.Object);

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
        public void RetornaNadaQuandoArquivoNaoForONNXParaBuscaModeloCommand(string? caminho)
        {
            //Arrange
            var mockGerenciador = new Mock<IGerenciador<ModeloDTO>>();
            var mockDialog = new Mock<IDialogService>();
            mockDialog.Setup(f => f.GetCaminhoArquivo()).Returns(caminho);

            var viewModel = new InserirModeloViewModel(_navigation, mockGerenciador.Object, mockDialog.Object, _mockConversor.Object);

            //Act
            viewModel.BuscaModeloCommand.Execute(null);
            string fakeCaminho = viewModel.CaminhoModelo;
            string fakeOnnx = viewModel.NomeModelo;


            //Assert
            Assert.Empty(fakeCaminho);
            Assert.Empty(fakeOnnx);
        }



    }
}
