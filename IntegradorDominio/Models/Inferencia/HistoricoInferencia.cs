namespace IntegradorDominio.Models.Inferencia
{
    public class HistoricoInferencia
    {
        public DateTime DataHora { get; set; }

        public string NomeModelo { get; set; }

        public string VersaoModelo { get; set; }

        public int TotalLinhas { get; set; }

        public int LinhasComErro { get; set; }

        public int LinhasProcessadas => TotalLinhas - LinhasComErro;

        public string TempoExecucaoMs { get; set; }

        public string Status { get; set; }
    }
}
