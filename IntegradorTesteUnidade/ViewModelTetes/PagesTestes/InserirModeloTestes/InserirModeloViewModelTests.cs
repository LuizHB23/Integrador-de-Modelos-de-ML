using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorAplicacao.Infraestrutura.Gerenciador;
using IntegradorDominio.Models.Configuracao;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.InserirModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using Moq;

namespace IntegradorTesteUnidade.ViewModelTetes.PagesTestes.InserirModeloTestes
{
    public class InserirModeloViewModelTests
    {
        private readonly Mock<IGerenciador<ModeloDTO>> _mockGerenciador;
        private readonly Mock<IConversorJson> _mockConversor;
        private readonly Mock<IDialogService> _mockDialog;
        private readonly Mock<INavigationService> _mockNavigation;
        private readonly Mock<IContext<ModeloDTO>> _mockContext;
        private readonly InserirModeloViewModel _viewModel;

        public InserirModeloViewModelTests()
        {
            _mockNavigation = new Mock<INavigationService>();
            _mockConversor = new Mock<IConversorJson>();
            _mockGerenciador = new Mock<IGerenciador<ModeloDTO>>();
            _mockDialog = new Mock<IDialogService>();
            _mockContext = new Mock<IContext<ModeloDTO>>();

            _viewModel = new InserirModeloViewModel(_mockNavigation.Object, _mockGerenciador.Object, _mockDialog.Object, _mockConversor.Object, _mockContext.Object);
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
        public async Task RetornaOkParaCamposVisitacosComSucessoNoFluxoEmNavigateToConfigurarSchemaCommand()
        {
            //Arrange
            _mockGerenciador.Setup(f => f.Salvar(It.IsAny<ModeloDTO>())).Returns("C://FakePath//modelo.onnx");

            _viewModel.NomeModelo = "Nome Qualquer";
            _viewModel.TipoModelo = "Regressão";
            _viewModel.CaminhoModelo = "Caminho Qualquer";

            //Act
            await _viewModel.NavigateToConfigurarSchemaCommand.ExecuteAsync(null);

            //Assert
            _mockContext.Verify(f => f.EnviaMensagem(It.IsAny<ModeloDTO>()), Times.Once);
            _mockGerenciador.Verify(f => f.Salvar(It.IsAny<ModeloDTO>()), Times.Once);
            _mockConversor.Verify(f => f.ConverteJsonAsync(It.IsAny<ModeloConfiguracao>()), Times.Once);
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
            _mockConversor.Verify(f => f.ConverteJsonAsync(It.IsAny<ModeloDTO>()), Times.Never);
            _mockDialog.Verify(f => f.ShowMessage($"Preencha corretamente os campos", "Campos Faltantes"), Times.Once);
        }

        [Fact]
        public void RetornaMensageBoxParaNomeModeloInvalidoEmNavigateToConfigurarSchemaCommand()
        {
            //Arrange
            _viewModel.NomeModelo = "@#$%¨&*(¨%&*&";
            _viewModel.TipoModelo = "Tipo Qualquer";
            _viewModel.CaminhoModelo = "Caminho Qualquer";

            string erro = "Ocorreu um erro aqui";
            _mockGerenciador.Setup(g => g.Salvar(It.IsAny<ModeloDTO>()))
                            .Throws(new IOException(erro));

            //Act
            _viewModel.NavigateToConfigurarSchemaCommand.Execute(null);

            //Assert
            _mockDialog.Verify(f => f.ShowMessage($"Nome Inválido: {erro}", "Erro"), Times.Once);
            _mockNavigation.Verify(f => f.NavigateTo<ConfigurarSchemaViewModel>(), Times.Never);
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
