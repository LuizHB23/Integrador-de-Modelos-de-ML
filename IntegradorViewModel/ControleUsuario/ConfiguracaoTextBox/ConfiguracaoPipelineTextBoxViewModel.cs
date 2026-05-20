using CommunityToolkit.Mvvm.ComponentModel;
using IntegradorViewModel.ControleUsuario.ConfiguracaoMetodo.EstadoDataFrame;
using System.Data;
using IntegradorViewModel.ControleUsuario.ConfiguracaoTextBox;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorViewModel.ControleUsuario
{
    public partial class ConfiguracaoPipelineTextBoxViewModel : ObservableObject, IConfiguracaoTextBox
    {
        [ObservableProperty]
        private DataView _dadosPreview;

        [ObservableProperty]
        private string _scriptCodigo;

        [ObservableProperty]
        private bool _dataFrameMudou;

        private EstadoDataFrameViewModel _estadoDataFrame;
        private ConfiguracaoTextBoxViewModel _textBox;

        public ConfiguracaoPipelineTextBoxViewModel(ConfiguracaoTextBoxViewModel textBox, DataView dadosPreview, EstadoDataFrameViewModel estadoDataFrame)
        {
            ScriptCodigo = string.Empty;
            DadosPreview = dadosPreview;
            DataFrameMudou = false;

            _estadoDataFrame = estadoDataFrame;
            _textBox = textBox;
        }

        public Dictionary<string, List<string>>? MandaCodigoMetodo() => _textBox.MandaCodigoMetodo(ScriptCodigo);
        public DataTable DataFrameParaDataTable(DataFrame dataFrame) => _textBox.DataFrameParaDataTable(dataFrame);
        public void AtualizaTabela(DataFrame dataFrame) => _textBox.AtualizaTabela(dataFrame);
        public void EscreveScript(string featureName, List<string> listaPropriedades) => ScriptCodigo = _textBox.EscreveScript(featureName, listaPropriedades, ScriptCodigo);
        public void EsvaziaScript() => ScriptCodigo = _textBox.EsvaziaScript();

        public DataFrame CarregarDados() => _estadoDataFrame.CarregarDados();
        public async Task GuardaEstado() => await _estadoDataFrame.GuardaEstadoArquivo();
    }
}
