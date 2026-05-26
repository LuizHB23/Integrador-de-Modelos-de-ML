using IntegradorDominio.Models.ModeloEtapas;

namespace IntegradorDominio.Models.Configuracao
{
    public class TransformadorConfiguracao
    {
        public string NomeModelo { get; set; }
        public string Versao { get; set; }
        public Dictionary<int, Transformador> Transformadores {  get; set; }

        public TransformadorConfiguracao(string nomeModelo, string versao, Dictionary<int, Transformador> transformadores)
        {
            NomeModelo = nomeModelo;
            Versao = versao;
            Transformadores = transformadores;
        }
    }
}
