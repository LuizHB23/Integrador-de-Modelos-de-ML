using IntegradorAplicacao.DTO;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.Shared.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace IntegradorTesteUnidade.ViewModelTetes.ControleUsuarioTestes
{
    public class ConfiguracaoMetodoTextBoxViewModelTests : IDisposable
    {
        private readonly Mock<IDialogService> _dialogMock = new();
        private readonly List<string> _arquivosTemp = new();

        private ConfiguracaoMetodoTextBoxViewModel CriarVM(Action<DataView>? callback = null)
        {
            var caminho = CriarCsvFake();

            return new ConfiguracaoMetodoTextBoxViewModel(
                _dialogMock.Object,
                new ArquivoDadosDTO(caminho, ',', "utf-8", '.', true),
                callback ?? (_ => { }),
                new DataView()
            );
        }

        // =========================
        // 🧪 MandaCodigoMetodo
        // =========================

        [Fact]
        public void MandaCodigoMetodo_RetornaNull_QuandoScriptVazio()
        {
            var vm = CriarVM();

            vm.ScriptCodigo = "";

            var result = vm.MandaCodigoMetodo();

            Assert.Null(result);
        }

        [Fact]
        public void MandaCodigoMetodo_MostraErro_QuandoScriptInvalido()
        {
            var vm = CriarVM();

            vm.ScriptCodigo = "codigo inválido !!!";

            var result = vm.MandaCodigoMetodo();

            Assert.Null(result);

            _dialogMock.Verify(x =>
                x.ShowMessage(It.Is<string>(s => s.Contains("Código do método está errado")), "Código Errado"),
                Times.Once);
        }

        // =========================
        // 🧪 CarregarDados
        // =========================

        [Fact]
        public void CarregarDados_CriaDataFrameCorretamente()
        {
            var vm = CriarVM();

            var df = vm.CarregarDados();

            Assert.Equal(2, df.Colunas.Count);
            Assert.Equal(2, df.QuantidadeLinhas);
        }

        // =========================
        // 🧪 AtualizaTabela
        // =========================

        [Fact]
        public void AtualizaTabela_ChamaCallbackComDataView()
        {
            DataView? recebido = null;

            var vm = CriarVM(dv => recebido = dv);

            var df = vm.CarregarDados();

            vm.AtualizaTabela(df);

            Assert.NotNull(recebido);
            Assert.True(recebido!.Count > 0);
        }

        // =========================
        // 🧪 EscreveScript
        // =========================

        [Fact]
        public void EscreveScript_CriaScriptPadrao_QuandoVazio()
        {
            var vm = CriarVM();

            vm.ScriptCodigo = "";

            vm.EscreveScript("Sum", new List<string> { "coluna" });

            Assert.Contains("SuaFuncao()", vm.ScriptCodigo);
        }

        [Fact]
        public void EscreveScript_AdicionaMetodoNoScript()
        {
            var vm = CriarVM();

            vm.ScriptCodigo = "MinhaFuncao()\n{\nreturn df\n}";

            vm.EscreveScript("Sum", new List<string> { "coluna" });

            Assert.Contains("df = df.Sum(", vm.ScriptCodigo);
        }

        [Fact]
        public void EscreveScript_Map_AdicionaLambdaEspecial()
        {
            var vm = CriarVM();

            vm.ScriptCodigo = "MinhaFuncao()\n{\nreturn df\n}";

            vm.EscreveScript("Map", new List<string>());

            Assert.Contains("lambdax", vm.ScriptCodigo);
        }

        [Fact]
        public void EscreveScript_IgnoraContexto()
        {
            var vm = CriarVM();

            vm.ScriptCodigo = "MinhaFuncao()\n{\nreturn df\n}";

            vm.EscreveScript("Sum", new List<string> { "col1", "Contexto", "col2" });

            Assert.Contains("col1=", vm.ScriptCodigo);
            Assert.Contains("col2=", vm.ScriptCodigo);
            Assert.DoesNotContain("Contexto=", vm.ScriptCodigo);
        }

        // =========================
        // 🧪 EsvaziaScript
        // =========================

        [Fact]
        public void EsvaziaScript_LimpaCodigo()
        {
            var vm = CriarVM();

            vm.ScriptCodigo = "algo";

            vm.EsvaziaScript();

            Assert.Equal(string.Empty, vm.ScriptCodigo);
        }

        // =========================
        // 🧪 Helpers
        // =========================

        private string CriarCsvFake()
        {
            var caminho = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");

            File.WriteAllLines(caminho, new[]
            {
            "col1,col2",
            "1,2",
            "3,4"
        });

            _arquivosTemp.Add(caminho);

            return caminho;
        }

        public void Dispose()
        {
            foreach (var arq in _arquivosTemp)
            {
                try
                {
                    if (File.Exists(arq))
                        File.Delete(arq);
                }
                catch { }
            }
        }
    }
}
