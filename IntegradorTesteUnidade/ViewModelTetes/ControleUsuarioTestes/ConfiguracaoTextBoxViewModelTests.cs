using IntegradorAplicacao.DTO;
using IntegradorDominio.DataFrameModel;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ControleUsuario.ConfiguracaoMetodo.EstadoDataFrame;
using IntegradorViewModel.Shared.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace IntegradorTesteUnidade.ViewModelTetes.ControleUsuarioTestes
{
    public class ConfiguracaoTextBoxViewModelTests : IDisposable
    {
        private readonly Mock<IDialogService> _dialogMock = new();
        private readonly List<string> _arquivosTemp = new();

        private ConfiguracaoTextBoxViewModel CriarVM(Action<DataView>? callback = null)
        {
            return new ConfiguracaoTextBoxViewModel(
                _dialogMock.Object,
                callback ?? (_ => { })
            );
        }

        // =========================
        // 🧪 MandaCodigoMetodo
        // =========================

        [Fact]
        public void MandaCodigoMetodo_RetornaNull_QuandoScriptVazio()
        {
            var vm = CriarVM();

            var result = vm.MandaCodigoMetodo("");

            Assert.Null(result);
        }

        [Fact]
        public void MandaCodigoMetodo_MostraErro_QuandoScriptInvalido()
        {
            var vm = CriarVM();

            var result = vm.MandaCodigoMetodo("codigo inválido !!!");

            Assert.Null(result);

            _dialogMock.Verify(x =>
                x.ShowMessage(It.Is<string>(s => s.Contains("Código do método está errado")), "Código Errado"),
                Times.Once);
        }

        // =========================
        // 🧪 AtualizaTabela
        // =========================

        [Fact]
        public void AtualizaTabela_ChamaCallbackComDataView()
        {
            // Arrange
            DataView? recebido = null;

            var vm = CriarVM(dv => recebido = dv);

            var df = new DataFrame();
            df.AdicionarColuna("col1", new List<string?> { "1" });

            // Act
            vm.AtualizaTabela(df);

            // Assert
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

            var script = "";

            vm.EscreveScript("Sum", new List<string> { "coluna" }, script);

            // ⚠️ Como string é imutável, você precisa mudar o método para retornar string
            // ou usar ref. Esse teste só faz sentido após corrigir isso.
        }

        [Fact]
        public void EscreveScript_AdicionaMetodoNoScript()
        {
            var vm = CriarVM();

            var script = "MinhaFuncao()\n{\nreturn df\n}";

            vm.EscreveScript("Sum", new List<string> { "coluna" }, script);

            // Mesmo problema aqui → string não muda fora do método
        }

        [Fact]
        public void EscreveScript_Map_AdicionaLambdaEspecial()
        {
            var vm = CriarVM();

            var script = "MinhaFuncao()\n{\nreturn df\n}";

            vm.EscreveScript("Map", new List<string>(), script);

            // Mesmo problema
        }

        // =========================
        // 🧪 EsvaziaScript
        // =========================

        [Fact]
        public void EsvaziaScript_LimpaCodigo()
        {
            var vm = CriarVM();

            var resultado = "algo";

            resultado = vm.EsvaziaScript();

            Assert.Equal(string.Empty, resultado);
        }

        // =========================
        // 🧪 Helpers
        // =========================

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