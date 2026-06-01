using IntegradorDominio.Models.Configuracao.Interfaces;
using IntegradorDominio.Models.ModeloEtapas;
using System.Text.Json.Serialization;

namespace IntegradorDominio.Models.Configuracao
{
    public class PipelineTratamentoConfiguracao : IListaConfiguracao, IPipelineConfiguracao
    {
        public string NomeModelo { get; set; }
        public string Versao { get; set; }
        public Dictionary<int, Pipeline> ScriptCodigo { get; set; }

        [JsonIgnore]
        public Dictionary<int, Pipeline> Dicionario
        {
            get => ScriptCodigo;

            set;
        }

        public PipelineTratamentoConfiguracao(string nomeModelo, string versao, Dictionary<int, Pipeline> scriptCodigo)
        {
            NomeModelo = nomeModelo;
            Versao = versao;
            ScriptCodigo = scriptCodigo;
        }
    }
}
