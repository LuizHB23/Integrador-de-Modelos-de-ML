using IntegradorDominio.Models.ModeloEtapas;

namespace IntegradorDominio.Models.Configuracao
{
    public class SchemaConfiguracao
    {
        public string NomeModelo { get; set; }
        public string Versao { get; set; }
        public Dictionary<int, Schema> Colunas {  get; set; }

        public SchemaConfiguracao(string nomeModelo, string versao, Dictionary<int, Schema> colunas)
        {
            NomeModelo = nomeModelo;
            Versao = versao;
            Colunas = colunas;
        }
    }
}
