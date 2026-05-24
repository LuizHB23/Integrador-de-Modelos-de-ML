using IntegradorAplicacao.DTO;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.InserirModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using Moq;
using System.Text;

namespace IntegradorTesteUnidade.ViewModelTetes.PagesTestes.InserirModeloTestes
{
    public class CarregarDadosViewModelTests
    {
        private readonly Mock<IDialogService> _dialogMock = new();
        private readonly Mock<INavigationService> _navigationMock = new();
        private readonly Mock<IContext<ArquivoDadosDTO>> _contextMock = new();

        private CarregarDadosViewModel CriarVM()
        {
            return new CarregarDadosViewModel(
                _navigationMock.Object,
                _dialogMock.Object,
                _contextMock.Object
            );
        }

        // =========================
        // 🧪 Construtor
        // =========================

        [Fact]
        public void Construtor_InicializaValoresPadrao()
        {
            var vm = CriarVM();

            Assert.Equal("Vírgula (,)", vm.Delimitador);
            Assert.Equal("UTF-8", vm.Codificacao);
            Assert.Equal("Ponto (.)", vm.PontuacaoDecimal);
            Assert.True(vm.ContemCabecalho);
        }

        // =========================
        // 🧪 CarregarArquivoDados
        // =========================

        [Fact]
        public void CarregarArquivoDados_ChamaDialog()
        {
            _dialogMock.Setup(x => x.GetCaminhoArquivo())
                .Returns("caminho.csv");

            var vm = CriarVM();

            vm.CarregarArquivoDadosCommand.Execute(null);

            _dialogMock.Verify(x => x.GetCaminhoArquivo(), Times.Once);
            Assert.Equal("caminho.csv", vm.CaminhoArquivoDados);
        }

        // =========================
        // 🧪 Regras Delimitador/Decimal
        // =========================

        [Fact]
        public void Delimitador_Virgula_AtualizaDecimalSeConflito()
        {
            var vm = CriarVM();

            vm.PontuacaoDecimal = "Vírgula (,)";
            vm.Delimitador = "Vírgula (,)";

            Assert.Equal("Ponto (.)", vm.PontuacaoDecimal);
        }

        [Fact]
        public void Decimal_Virgula_AtualizaDelimitadorSeConflito()
        {
            var vm = CriarVM();

            vm.Delimitador = "Vírgula (,)";
            vm.PontuacaoDecimal = "Vírgula (,)";

            Assert.Equal("Ponto e Vírgula (;)", vm.Delimitador);
        }

        // =========================
        // 🧪 NavigateToPipelineModelo
        // =========================

        [Fact]
        public void NavigateToPipeline_ComCaminho_EnviaContextoENavega()
        {
            var vm = CriarVM();

            vm.CaminhoArquivoDados = "arquivo.csv";

            vm.NavigateToPipelineModeloCommand.Execute(null);

            _contextMock.Verify(x => x.EnviaMensagem(It.IsAny<ArquivoDadosDTO>()), Times.Once);
            _navigationMock.Verify(x => x.NavigateTo<PipelineModeloViewModel>(), Times.Once);
        }

        [Fact]
        public void NavigateToPipeline_SemCaminho_MostraMensagemENaoNavega()
        {
            var vm = CriarVM();

            vm.CaminhoArquivoDados = "";

            vm.NavigateToPipelineModeloCommand.Execute(null);

            _dialogMock.Verify(x =>
                x.ShowMessage("Precisa-se de um arquivo prévio", "Caminho Vazio"),
                Times.Once);

            // ⚠️ comportamento atual: ainda navega
            _navigationMock.Verify(x => x.NavigateTo<PipelineModeloViewModel>(), Times.Never);
        }

        // =========================
        // 🧪 NavigateToHome
        // =========================

        [Fact]
        public void NavigateToHome_FinalizaFluxoENavega()
        {
            var vm = CriarVM();

            vm.NavigateToHomeCommand.Execute(null);

            _navigationMock.Verify(x => x.EndFlow(), Times.Once);
            _navigationMock.Verify(x => x.NavigateTo<HomeViewModel>(), Times.Once);
        }
    }
}