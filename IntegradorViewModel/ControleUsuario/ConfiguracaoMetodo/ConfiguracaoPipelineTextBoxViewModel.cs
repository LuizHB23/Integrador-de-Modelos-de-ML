using CommunityToolkit.Mvvm.ComponentModel;
using IntegradorViewModel.ControleUsuario.ConfiguracaoMetodo.EstadoDataFrame;
using IntegradorDominio.DataFrameModel;
using System.Data;

namespace IntegradorViewModel.ControleUsuario
{
    public partial class ConfiguracaoPipelineTextBoxViewModel : ObservableObject
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
        public void EscreveScript(string featureName, List<string> listaPropriedades) => _textBox.EscreveScript(featureName, listaPropriedades, ScriptCodigo);
        public void EsvaziaScript() => _textBox.EsvaziaScript(ScriptCodigo);

        public DataFrame CarregarDados() => _estadoDataFrame.CarregarDados();
        public async Task GuardaEstadoArquivo() => await _estadoDataFrame.GuardaEstadoArquivo();
    }
}
