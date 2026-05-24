using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Gerenciador;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.InserirModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using System.Reflection;
using Moq;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;

namespace IntegradorTesteUnidade.ViewModelTetes.PagesTestes.InserirModeloTestes
{
    public class TransformadoresModeloViewModelTests
    {
        [Fact]
        public void NaoDeveAdicionarTransformador_SeCamposVazios()
        {
            var dialogMock = new Mock<IDialogService>();
            var vm = CriarViewModel(dialogMock: dialogMock);

            vm.NomeTransformador = "";
            vm.CaminhoTransformador = "";

            vm.AdicionaTransformadorCommand.Execute(null);

            dialogMock.Verify(d => d.ShowMessage(
                It.IsAny<string>(),
                It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public void DeveAdicionarTransformador_QuandoCamposValidos()
        {
            var gerenciadorMock = new Mock<IGerenciador<TransformadorDTO>>();

            gerenciadorMock
                .Setup(g => g.Salvar(It.IsAny<TransformadorDTO>()))
                .Returns("caminho_final");

            var vm = CriarViewModel(gerenciadorMock: gerenciadorMock);

            vm.NomeTransformador = "Teste";
            vm.CaminhoTransformador = "arquivo.onnx";

            SetCampoPrivado(vm, "_caminhoProvisorio", "caminho_completo.onnx");

            vm.AdicionaTransformadorCommand.Execute(null);

            Assert.Single(vm.CardsTransformador);
        }

        [Fact]
        public void DeveChamarGerenciador_AoAdicionar()
        {
            var gerenciadorMock = new Mock<IGerenciador<TransformadorDTO>>();

            gerenciadorMock
                .Setup(g => g.Salvar(It.IsAny<TransformadorDTO>()))
                .Returns("caminho_final");

            var vm = CriarViewModel(gerenciadorMock: gerenciadorMock);

            vm.NomeTransformador = "Teste";
            vm.CaminhoTransformador = "arquivo.onnx";

            SetCampoPrivado(vm, "_caminhoProvisorio", "caminho_completo.onnx");

            vm.AdicionaTransformadorCommand.Execute(null);

            gerenciadorMock.Verify(g => g.Salvar(It.IsAny<TransformadorDTO>()), Times.Once);
        }

        [Fact]
        public void DeveCarregarCaminho_OnnxValido()
        {
            var dialogMock = new Mock<IDialogService>();

            dialogMock
                .Setup(d => d.GetCaminhoArquivo())
                .Returns("C:\\modelos\\teste.onnx");

            var vm = CriarViewModel(dialogMock: dialogMock);

            vm.CarregarCaminhoTransformadorOnnxCommand.Execute(null);

            Assert.Equal("teste.onnx", vm.CaminhoTransformador);
        }

        [Fact]
        public void NaoDeveCarregarCaminho_ArquivoInvalido()
        {
            var dialogMock = new Mock<IDialogService>();

            dialogMock
                .Setup(d => d.GetCaminhoArquivo())
                .Returns("C:\\modelos\\teste.txt");

            var vm = CriarViewModel(dialogMock: dialogMock);

            vm.CarregarCaminhoTransformadorOnnxCommand.Execute(null);

            Assert.True(string.IsNullOrEmpty(vm.CaminhoTransformador));
        }

        [Fact]
        public async Task DeveNavegarParaHome()
        {
            var navigationMock = new Mock<INavigationService>();

            var vm = CriarViewModel(navigationMock: navigationMock);

            await vm.NavigateToHomeCommand.ExecuteAsync(null);

            navigationMock.Verify(n => n.EndFlow(), Times.Once);
            navigationMock.Verify(n => n.NavigateTo<HomeViewModel>(), Times.Once);
        }

        private TransformadoresModeloViewModel CriarViewModel(
            Mock<INavigationService> navigationMock = null,
            Mock<IDialogService> dialogMock = null,
            Mock<IConversorJson> converterMock = null,
            Mock<IContext<ModeloDTO>> contextMock = null,
            Mock<IPathProvider> providerMock = null,
            Mock<IGerenciador<TransformadorDTO>> gerenciadorMock = null)
        {
            navigationMock ??= new Mock<INavigationService>();
            dialogMock ??= new Mock<IDialogService>();
            converterMock ??= new Mock<IConversorJson>();
            contextMock ??= new Mock<IContext<ModeloDTO>>();
            providerMock ??= new Mock<IPathProvider>();
            gerenciadorMock ??= new Mock<IGerenciador<TransformadorDTO>>();

            contextMock
                .Setup(c => c.RecebeMensagem())
                .Returns(new ModeloDTO("modelo_teste", "", "", "1.0"));

            return new TransformadoresModeloViewModel(
                navigationMock.Object,
                dialogMock.Object,
                converterMock.Object,
                contextMock.Object,
                providerMock.Object,
                gerenciadorMock.Object
            );
        }

        private void SetCampoPrivado(object obj, string nomeCampo, object valor)
        {
            var field = obj.GetType()
                .GetField(nomeCampo, BindingFlags.NonPublic | BindingFlags.Instance);

            field.SetValue(obj, valor);
        }
    }
}
