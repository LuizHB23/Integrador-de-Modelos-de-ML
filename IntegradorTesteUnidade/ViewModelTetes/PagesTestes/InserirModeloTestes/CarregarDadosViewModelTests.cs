using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.InserirModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using IntegradorViewModel.Shared.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.ViewModelTetes.PagesTestes.InserirModeloTestes
{
    public class CarregarDadosViewModelTests
    {
        private readonly Mock<IDialogService> _mockDialog;
        private readonly Mock<INavigationService> _mockNavigation;

        public CarregarDadosViewModelTests()
        {
            _mockNavigation = new Mock<INavigationService>();
            _mockDialog = new Mock<IDialogService>();
        }

        [Fact]
        public void RetornaEhIgualEVerdadeiroParaVariaveisCarregasEmConstrutor()
        {
            //Arrange
            var delimitador = "Vírgula (,)";
            var codificacao = "UTF-8";
            var decimalVariavel = "Ponto (.)";

            //Act
            var viewModel = new CarregarDadosViewModel(_mockNavigation.Object, _mockDialog.Object);

            //Assert
            Assert.Equal(delimitador, viewModel.Delimitador);
            Assert.Equal(codificacao, viewModel.Codificacao);
            Assert.Equal(decimalVariavel, viewModel.Decimal);
            Assert.True(viewModel.ContemCabecalho);
        }

        [Fact]
        public void RetornaOkParaDialogAcessadoEmCarregarArquivoDados()
        {
            //Arrange
            _mockDialog.Setup(f => f.GetCaminhoArquivo()).Returns("");
            var viewModel = new CarregarDadosViewModel(_mockNavigation.Object, _mockDialog.Object);

            //Act
            viewModel.CarregarArquivoDadosCommand.Execute(null);

            //Assert
            _mockDialog.Verify(f => f.GetCaminhoArquivo(), Times.Once);
        }

        [Fact]
        public void RetornaEhIgualParaAtualizacaoVariavelDelimitadorQuandoDecimalIgualAoDelimitadorEmCarregarArquivoDados()
        {
            //Arrange
            string delimitador = "Ponto e Vírgula (;)";
            var viewModel = new CarregarDadosViewModel(_mockNavigation.Object, _mockDialog.Object);

            //Act
            viewModel.Decimal = "Vírgula (,)";

            //Assert
            Assert.Equal(delimitador, viewModel.Delimitador);
        }

        [Fact]
        public void RetornaEhIgualParaAtualizacaoVariavelDecimalQuandoDelimitadorIgualAoDecimalEmCarregarArquivoDados()
        {
            //Arrange
            string decimalVariavel = "Ponto (.)";
            var viewModel = new CarregarDadosViewModel(_mockNavigation.Object, _mockDialog.Object);

            viewModel.Delimitador = "Qualquer coisa";
            viewModel.Decimal = "Vírgula (,)";

            //Act
            viewModel.Delimitador = "Vírgula (,)";

            //Assert
            Assert.Equal(decimalVariavel, viewModel.Decimal);
        }

        [Fact]
        public void RetornaOkParaNavegacaoRealizadaComSucessoEmNavigateToPipelineModeloCommand()
        {
            //Arrange
            var viewModel = new CarregarDadosViewModel(_mockNavigation.Object, _mockDialog.Object);
            viewModel.CaminhoArquivoDados = "Caminho Qualquer";

            //Act
            viewModel.NavigateToPipelineModeloCommand.Execute(null);

            //Assert
            _mockNavigation.Verify(f => f.NavigateTo<PipelineModeloViewModel>(), Times.Once);
        }

        [Fact]
        public void RetornaMensageBoxParaCaminhoArquivoDadosNaoPreenchidoEmNavigateToPipelineModeloCommand()
        {
            //Arrange
            var viewModel = new CarregarDadosViewModel(_mockNavigation.Object, _mockDialog.Object);

            //Act
            viewModel.NavigateToPipelineModeloCommand.Execute(null);

            //Assert
            _mockDialog.Verify(f => f.ShowMessage("Precisa-se de um arquivo prévio", "Schema Vazio"), Times.Once);
            _mockNavigation.Verify(f => f.NavigateTo<PipelineModeloViewModel>(), Times.Never);
        }

        [Fact]
        public void RetornaOkParaFluxoScopedEncerradoERetonoHomeEmNavigateToHomeCommand()
        {
            //Arrange
            var viewModel = new CarregarDadosViewModel(_mockNavigation.Object, _mockDialog.Object);

            //Act
            viewModel.NavigateToHomeCommand.Execute(null);

            //Assert
            _mockNavigation.Verify(f => f.EndFlow(), Times.Once);
            _mockNavigation.Verify(f => f.NavigateTo<HomeViewModel>(), Times.Once);
        }
    }
}
