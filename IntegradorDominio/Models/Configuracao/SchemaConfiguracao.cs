using IntegradorDominio.Models.Configuracao.Interfaces;
using IntegradorDominio.Models.ModeloEtapas;
using System.Text.Json.Serialization;

namespace IntegradorDominio.Models.Configuracao
{
    public class SchemaConfiguracao : IListaConfiguracao
    {
        public string NomeModelo { get; set; }
        public string Versao { get; set; }
        public Dictionary<int, Schema> Colunas {  get; set; }

        [JsonIgnore]
        public Dictionary<int, Schema> Dicionario { get => Colunas; set; }

        public SchemaConfiguracao(string nomeModelo, string versao, Dictionary<int, Schema> colunas)
        {
            NomeModelo = nomeModelo;
            Versao = versao;
            Colunas = colunas;
        }
    }
}
