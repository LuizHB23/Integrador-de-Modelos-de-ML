using IntegradorDominio.Models.Configuracao.Interfaces;
using IntegradorDominio.Models.ModeloEtapas;
using System.Text.Json.Serialization;

namespace IntegradorDominio.Models.Configuracao
{
    public class TransformadorConfiguracao : IListaConfiguracao
    {
        public string NomeModelo { get; set; }
        public string Versao { get; set; }
        public Dictionary<int, Transformador> Transformadores {  get; set; }

        [JsonIgnore]
        public Dictionary<int, Transformador> Dicionario { get => Transformadores; set; }

        public TransformadorConfiguracao(string nomeModelo, string versao, Dictionary<int, Transformador> transformadores)
        {
            NomeModelo = nomeModelo;
            Versao = versao;
            Transformadores = transformadores;
        }
    }
}
