using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.InserirModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using System.Data;
using System.Reflection;
using Moq;

namespace IntegradorTesteUnidade.ViewModelTetes.PagesTestes.InserirModeloTestes
{
    public class PipelineModeloViewModelTests : IDisposable
    {
        private readonly Mock<INavigationService> _navigationMock = new();
        private readonly Mock<IDialogService> _dialogMock = new();
        private readonly Mock<IConverteJson<Dictionary<int, FuncaoDTO>>> _converterMock = new();
        private readonly Mock<IContext<ModeloDTO>> _contextModeloMock = new();
        private readonly Mock<IContext<ArquivoDadosDTO>> _contextArquivoMock = new();
        private readonly Mock<IPathProvider> _pathProviderMock = new();

        private readonly List<string> _arquivosTemporarios = new();

        private PipelineModeloViewModel CriarViewModel()
        {
            var caminhoCsv = CriarCsvFake();

            _contextModeloMock.Setup(x => x.RecebeMensagem())
                .Returns(new ModeloDTO("modelo_teste", "", ""));

            _contextArquivoMock.Setup(x => x.RecebeMensagem())
                .Returns(new ArquivoDadosDTO(caminhoCsv, ',', "utf-8", '.', true));

            return new PipelineModeloViewModel(
                _navigationMock.Object,
                _dialogMock.Object,
                _converterMock.Object,
                _contextModeloMock.Object,
                _contextArquivoMock.Object,
                _pathProviderMock.Object
            );
        }

        private Mock<ConfiguracaoMetodoTextBoxViewModel> CriarTextBoxMock()
        {
            var caminhoCsv = CriarCsvFake();

            return new Mock<ConfiguracaoMetodoTextBoxViewModel>(
                _dialogMock.Object,
                new ArquivoDadosDTO(caminhoCsv, ',', "utf-8", '.', true),
                It.IsAny<Action<DataView>>(),
                new DataView()
            );
        }

        [Fact]
        public void AdicionaFuncao_NaoAdiciona_QuandoCodigoVazio()
        {
            var vm = CriarViewModel();

            var textBox = CriarTextBoxReal();
            textBox.ScriptCodigo = "";

            vm.TextBox = textBox;

            vm.AdicionaFuncao();

            Assert.Empty(vm.CardsFuncoes);
        }

        [Fact]
        public void AdicionaFuncao_AdicionaCard_QuandoValido()
        {
            var vm = CriarViewModel();

            _pathProviderMock.Setup(x => x.GetCaminhoModelo())
                .Returns(Path.GetTempPath());

            var textBox = CriarTextBoxReal();

            // script válido simples (parser aceita)
            textBox.ScriptCodigo = "MetodoTeste()\n{\nreturn df\n}";

            vm.TextBox = textBox;

            vm.AdicionaFuncao();

            Assert.Single(vm.CardsFuncoes);
        }

        [Fact]
        public void AdicionaFuncao_MostraErro_QuandoPipelineFalha()
        {
            var vm = CriarViewModel();

            _pathProviderMock.Setup(x => x.GetCaminhoModelo())
                .Returns(Path.GetTempPath());

            // força erro no pipeline (não no parser)
            _converterMock.Setup(x => x.CarregarJson(It.IsAny<string>()))
                .Throws(new Exception("erro pipeline"));

            var textBox = CriarTextBoxReal();

            // ✅ código válido
            textBox.ScriptCodigo = "MetodoTeste()\n{\nreturn df\n}";

            vm.TextBox = textBox;

            vm.AdicionaFuncao();

            _dialogMock.Verify(x =>
                x.ShowMessage(It.Is<string>(s => s.Contains("erro pipeline")), "Erro de Comando"),
                Times.Once);
        }

        [Fact]
        public void AtualizaFuncao_MostraMensagem_QuandoNaoExisteMetodo()
        {
            var vm = CriarViewModel();

            _pathProviderMock.Setup(x => x.GetCaminhoModelo())
                .Returns(Path.GetTempPath()); // ✅ CORREÇÃO

            var textBox = CriarTextBoxReal();
            textBox.ScriptCodigo = "MetodoInexistente()\n{\nreturn df\n}";

            vm.TextBox = textBox;

            _converterMock.Setup(x => x.CarregarJson(It.IsAny<string>()))
                .Returns(new Dictionary<int, FuncaoDTO>());

            vm.AtualizaFuncao();

            _dialogMock.Verify(x =>
                x.ShowMessage("Não há método para sobrevescrever"),
                Times.Once);
        }

        [Fact]
        public void NavigateToHome_ChamaNavegacaoCorreta()
        {
            var vm = CriarViewModel();

            vm.NavigateToHome();

            _navigationMock.Verify(x => x.EndFlow(), Times.Once);
            _navigationMock.Verify(x => x.NavigateTo<HomeViewModel>(), Times.Once);
        }

        [Fact]
        public void NavigateToTransformers_ChamaNavegacao()
        {
            var vm = CriarViewModel();

            vm.NavigateToTransformers();

            _navigationMock.Verify(x => x.NavigateTo<TransformadoresModeloViewModel>(), Times.Once);
        }

        [Fact]
        public void AlterouTabela_AtualizaDataPreview()
        {
            var vm = CriarViewModel();

            var dataTable = new DataTable();
            dataTable.Columns.Add("A");
            dataTable.Rows.Add("1");

            var dataView = new DataView(dataTable);

            vm.AlterouTabela(dataView);

            Assert.Equal(dataView, vm.DataPreview);
        }

        [Fact]
        public void ConfigurarFuncao_PreencheScriptCorretamente()
        {
            var vm = CriarViewModel();

            var card = CriarCardFake(nome: "MetodoTeste");

            _pathProviderMock.Setup(x => x.GetCaminhoModelo())
                .Returns(Path.GetTempPath());

            _converterMock.Setup(x => x.CarregarJson(It.IsAny<string>()))
                .Returns(new Dictionary<int, FuncaoDTO>
                {
            {
                1,
                new FuncaoDTO("MetodoTeste", new List<string> { "linha1", "linha2" }, "modelo")
            }
                });

            vm.ConfigurarFuncao(card);

            Assert.Contains("MetodoTeste()", vm.TextBox.ScriptCodigo);
            Assert.Contains("linha1", vm.TextBox.ScriptCodigo);
            Assert.Contains("linha2", vm.TextBox.ScriptCodigo);
        }

        [Fact]
        public void RemoverColuna_RemoveCard()
        {
            var vm = CriarViewModel();

            var card = CriarCardFake();

            vm.CardsFuncoes.Add(card);

            var metodo = typeof(PipelineModeloViewModel)
                .GetMethod("RemoverColuna", BindingFlags.NonPublic | BindingFlags.Instance);

            metodo.Invoke(vm, new object[] { card });

            Assert.Empty(vm.CardsFuncoes);
        }

        [Fact]
        public void OrganizaPosicao_NaoLancaExcecao()
        {
            var vm = CriarViewModel();

            var card = CriarCardFake();

            vm.CardsFuncoes.Add(card);

            var metodo = typeof(PipelineModeloViewModel)
            .GetMethod("OrganizaPosicao", BindingFlags.NonPublic | BindingFlags.Instance);

            var ex = Record.Exception(() =>
                metodo.Invoke(vm, new object[] { card, 0 })
            );

            Assert.Null(ex);
        }

        private ConfiguracaoMetodoTextBoxViewModel CriarTextBoxReal()
        {
            var caminhoCsv = CriarCsvFake();

            return new ConfiguracaoMetodoTextBoxViewModel(
                _dialogMock.Object,
                new ArquivoDadosDTO(
                    caminhoCsv,
                    ',',
                    "utf-8",
                    '.',
                    true
                ),
                _ => { },
                new DataView()
            );
        }

        private ConfiguracaoCardFuncaoViewModel CriarCardFake(int posicao = 1, string nome = "MetodoTeste")
        {
            var funcaoItem = new FuncaoItemViewModel(posicao, nome, new List<string>());

            return new ConfiguracaoCardFuncaoViewModel(
                funcaoItem,
                _ => { }, // ação remover
                (_, __) => { }, // ação trocar posição
                _ => { } // ação configurar
            );
        }

        private string CriarCsvFake()
        {
            var caminho = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");

            File.WriteAllLines(caminho, new[]
            {
            "col1,col2",
            "1,2",
            "3,4"
        });

            _arquivosTemporarios.Add(caminho);

            return caminho;
        }

        public void Dispose()
        {
            foreach (var arquivo in _arquivosTemporarios)
            {
                try
                {
                    if (File.Exists(arquivo))
                        File.Delete(arquivo);
                }
                catch
                {
                    // evita falha no teste por lock de arquivo
                }
            }
        }
    }
}
