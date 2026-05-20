namespace IntegradorDominio.Models.Configuracao
{
    public class ModeloConfiguracao
    {
        public string NomeModelo {  get; set; }
        public string Tipo {  get; set; }
        public string CaminhoPasta {  get; set; }
        public string Versao { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime AtualizadoEm { get; set; }
        public string SchemaVersao { get; set; }
        public string PipelineVersao { get; set; }
        public string TransformadoresVersao { get; set; }

        public ModeloConfiguracao(string nomeModelo, string tipo, string caminhoPasta)
        {
            NomeModelo = nomeModelo;
            Tipo = tipo;
            CaminhoPasta = caminhoPasta;
            Versao = "1.0";
            CriadoEm = DateTime.Now;
            AtualizadoEm = DateTime.Now;
        }

    }
}
