namespace IntegradorAplicacao.DTO
{
    public record ModeloDTO(string NomeModelo, string Tipo, string CaminhoPasta, string Versao)
    {
        public string CaminhoPasta { get; set; } = CaminhoPasta;
    }
}

