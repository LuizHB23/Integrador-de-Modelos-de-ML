using IntegradorDominio.Models.DataFrameModel;
using IntegradorDominio.Models.ModeloEtapas;

namespace IntegradorDominio.Models.Configuracao
{
    public class PipelineConfiguracao
    {
        public string NomeModelo { get; set; }
        public string Versao { get; set; }
        public Dictionary<int, PipelineTratamento> ScriptCodigo { get; set; }

        public PipelineConfiguracao(string nomeModelo, string versao, Dictionary<int, PipelineTratamento> scriptCodigo)
        {
            NomeModelo = nomeModelo;
            Versao = versao;
            ScriptCodigo = scriptCodigo;
        }
    }
}
