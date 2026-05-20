namespace IntegradorDominio.Models.Inferencia
{
    public class ResultadoInferencia
    {
        public string Id { get; set; }
        public Dictionary<string, float[]> Outputs { get; set; }
    }
}
