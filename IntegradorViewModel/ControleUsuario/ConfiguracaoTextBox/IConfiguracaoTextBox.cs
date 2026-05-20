using IntegradorDominio.Models.DataFrameModel;
using System.Data;

namespace IntegradorViewModel.ControleUsuario.ConfiguracaoTextBox
{
    public interface IConfiguracaoTextBox
    {
        public string ScriptCodigo { get; set; }

        public Dictionary<string, List<string>>? MandaCodigoMetodo();
        public DataTable DataFrameParaDataTable(DataFrame dataFrame);
        public void AtualizaTabela(DataFrame dataFrame);
        public void EscreveScript(string featureName, List<string> listaPropriedades);
        public void EsvaziaScript();

        public DataFrame CarregarDados();
        public Task GuardaEstado();
    }
}
