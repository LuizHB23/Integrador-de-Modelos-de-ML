using IntegradorDominio.Models.Configuracao.Interfaces;

namespace IntegradorDominio.Models.Configuracao
{
    public class ModeloConfiguracao : IListaConfiguracao
    {
        public string NomeModelo { get; set; }
        public string Versao { get; set; }
        public string Tipo { get; set; }
        public string CaminhoPasta { get; set; }

    }
}
