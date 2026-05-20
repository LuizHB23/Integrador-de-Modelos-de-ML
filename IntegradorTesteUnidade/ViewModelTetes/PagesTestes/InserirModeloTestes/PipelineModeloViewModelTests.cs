using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.ConversorJSON;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ControleUsuario.ConfiguracaoMetodo.EstadoDataFrame;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.InserirModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using Moq;
using System.Data;
using System.Reflection;
using System.Text;

namespace IntegradorTesteUnidade.ViewModelTetes.PagesTestes.InserirModeloTestes
{
    public class PipelineModeloViewModelTests
    {
        private readonly Mock<INavigationService> _navigation = new();
        private readonly Mock<IDialogService> _dialog = new();
        private readonly Mock<IConverteJson<Dictionary<int, FuncaoDTO>>> _converter = new();
        private readonly Mock<IContext<ModeloDTO>> _contextModelo = new();
        private readonly Mock<IContext<ArquivoDadosDTO>> _contextArquivo = new();
        private readonly Mock<IPathProvider> _path = new();

        private PipelineModeloViewModel CreateVM()
        {
            _contextModelo.Setup(x => x.RecebeMensagem())
                .Returns(new ModeloDTO("modelo", "", ""));

            _contextArquivo.Setup(x => x.RecebeMensagem())
                .Returns(new ArquivoDadosDTO("fake.csv", ',', Encoding.UTF8, '.', true));

            return new PipelineModeloViewModel(
                _navigation.Object,
                _dialog.Object,
                _converter.Object,
                _contextModelo.Object,
                _contextArquivo.Object,
                _path.Object
            );
        }

        // =========================
        // NAVIGATION
        // =========================

        [Fact]
        public void NavigateToHome_DeveFinalizarFluxoENavegar()
        {
            var vm = CreateVM();

            vm.NavigateToHome();

            _navigation.Verify(x => x.EndFlow(), Times.Once);
            _navigation.Verify(x => x.NavigateTo<HomeViewModel>(), Times.Once);
        }

        [Fact]
        public void NavigateToTransformers_DeveNavegarCorretamente()
        {
            var vm = CreateVM();

            vm.NavigateToTransformers();

            _navigation.Verify(x =>
                x.NavigateTo<TransformadoresModeloViewModel>(),
                Times.Once);
        }

        // =========================
        // DATA PREVIEW
        // =========================

        [Fact]
        public void AlterouTabela_DeveAtualizarDataPreview()
        {
            var vm = CreateVM();

            var table = new DataTable();
            table.Columns.Add("A");
            table.Rows.Add("1");

            var view = new DataView(table);

            vm.AlterouTabela(view);

            Assert.Equal(view, vm.DataPreview);
        }

        // =========================
        // INITIAL STATE
        // =========================

        [Fact]
        public void Criacao_ViewModel_DeveInicializarCollections()
        {
            var vm = CreateVM();

            Assert.NotNull(vm.CardsFuncoes);
            Assert.NotNull(vm.ListaFeatureEngineering);
            Assert.NotNull(vm.ListaTransformDataView);
            Assert.NotNull(vm.OpcoesPosicao);
            Assert.NotNull(vm.DataPreview);
            Assert.NotNull(vm.TextBox);
        }

        // =========================
        // SCRIPT MANAGER (ORQUESTRAÇÃO)
        // =========================
        // Aqui a regra correta:
        // NÃO testar lógica interna, só garantir que chama
        // (ScriptExecutorPipelineModeloManager deve ser testado separado)

        [Fact]
        public async Task AdicionaFuncao_DeveExecutarSemErro()
        {
            var vm = CreateVM();

            var ex = await Record.ExceptionAsync(async () =>
            {
                await vm.AdicionaFuncao();
            });

            Assert.Null(ex);
        }

        [Fact]
        public async Task AtualizaFuncao_DeveExecutarSemErro()
        {
            var vm = CreateVM();

            var ex = await Record.ExceptionAsync(async () =>
            {
                await vm.AtualizaFuncao();
            });

            Assert.Null(ex);
        }

        [Fact]
        public async Task CarregarPipeline_DeveExecutarSemErro()
        {
            var vm = CreateVM();

            var ex = await Record.ExceptionAsync(async () =>
            {
                await vm.CarregarPipeline();
            });

            Assert.Null(ex);
        }
    }
}