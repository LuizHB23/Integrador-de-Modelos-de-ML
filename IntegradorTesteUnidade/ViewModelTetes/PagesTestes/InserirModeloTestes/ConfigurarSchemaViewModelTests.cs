using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Gerenciador;
using IntegradorViewModel.Context;
using IntegradorViewModel.Interfaces;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.InserirModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.ViewModelTetes.PagesTestes.InserirModeloTestes
{
    public class ConfigurarSchemaViewModelTests
    {
        private readonly Mock<IConverteJson<Dictionary<int, SchemaDTO>>> _mockConversor;
        private readonly Mock<IDialogService> _mockDialog;
        private readonly Mock<INavigationService> _mockNavigation;
        private readonly Mock<IContext<ModeloDTO>> _mockContext;
        private readonly Mock<IPathProvider> _mockProvider;

        public ConfigurarSchemaViewModelTests()
        {
            _mockConversor = new Mock<IConverteJson<Dictionary<int, SchemaDTO>>>();
            _mockDialog = new Mock<IDialogService>();
            _mockNavigation = new Mock<INavigationService>();
            _mockContext = new Mock<IContext<ModeloDTO>>();
            _mockProvider = new Mock<IPathProvider>();
        }






        [Fact]
        public void RetornaOkParaFluxoScopedEncerradoERetonoHomeEmNavigateToHomeCommand()
        {
            //Arrange
            _mockContext.Setup(f => f.RecebeMensagem()).Returns(new ModeloDTO("Nome Qualquer", "", ""));

            var viewModel = new ConfigurarSchemaViewModel(_mockNavigation.Object, _mockDialog.Object, _mockConversor.Object, _mockContext.Object, _mockProvider.Object);

            //Act
            viewModel.NavigateToHomeCommand.Execute(null);

            //Assert
            _mockNavigation.Verify(f => f.EndFlow(), Times.Once);
            _mockNavigation.Verify(f => f.NavigateTo<HomeViewModel>(), Times.Once);
        }
    }
}
