using CommunityToolkit.Mvvm.ComponentModel;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.Inferencia;
using IntegradorViewModel.ControleUsuario.ConfiguracaoMetodo.EstadoDataFrame;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace IntegradorViewModel.ControleUsuario
{
    public partial class ConfiguracaoResultadoTextBoxViewModel : ObservableObject
    {
        [ObservableProperty]
        private DataView _dadosPreview;

        [ObservableProperty]
        private string _scriptCodigo;

        [ObservableProperty]
        private bool _dataFrameMudou;

        private EstadoDataFrameViewModel _estadoDataFrame;
        private ConfiguracaoTextBoxViewModel _textBox;

        public ConfiguracaoResultadoTextBoxViewModel(ConfiguracaoTextBoxViewModel textBox, DataView dadosPreview, EstadoDataFrameViewModel estadoDataFrame)
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
        public void EsvaziaScript() => ScriptCodigo = _textBox.EsvaziaScript();


        public DataFrame CarregarDados() => _estadoDataFrame.CarregarDados();
        public async Task GuardaEstadoResultado(List<ResultadoInferencia> resultados) => await _estadoDataFrame.GuardaEstadoResultado(resultados);
    }
}
