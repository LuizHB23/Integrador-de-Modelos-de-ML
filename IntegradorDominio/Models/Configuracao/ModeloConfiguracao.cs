using IntegradorDominio.Models.Enums;
using System.Text.Json.Serialization;

namespace IntegradorDominio.Models.Configuracao
{
    public class ModeloConfiguracao
    {
        public string NomeModelo {  get; set; }
        public TipoModelo Tipo {  get; set; }
        public string CaminhoPasta {  get; set; }
        public string Versao { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime AtualizadoEm { get; set; }
        public string SchemaVersao { get; set; }
        public string PipelineVersao { get; set; }
        public string TransformadoresVersao { get; set; }

        [JsonConstructor]
        public ModeloConfiguracao(string nomeModelo, TipoModelo tipo, string caminhoPasta, string versao, DateTime criadoEm, DateTime atualizadoEm, string schemaVersao, string pipelineVersao, string transformadoresVersao)
        {
            NomeModelo = nomeModelo;
            Tipo = tipo;
            CaminhoPasta = caminhoPasta;
            Versao = versao;
            CriadoEm = criadoEm;
            AtualizadoEm = atualizadoEm;
            SchemaVersao = schemaVersao;
            PipelineVersao = pipelineVersao;
            TransformadoresVersao = transformadoresVersao;
        }

        public ModeloConfiguracao(string nomeModelo, TipoModelo tipo, string caminhoPasta)
        {
            NomeModelo = nomeModelo;
            Tipo = tipo;
            CaminhoPasta = caminhoPasta;
            Versao = "1.0";
            CriadoEm = DateTime.Now;
            AtualizadoEm = DateTime.Now;
            SchemaVersao = string.Empty;
            PipelineVersao = string.Empty;
            TransformadoresVersao = string.Empty;
        }

    }
}
