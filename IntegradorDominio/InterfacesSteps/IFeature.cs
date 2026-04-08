namespace IntegradorDominio.InterfacesSteps
{
    public interface IFeature 
    {
        public string NomeExibicao { get; }
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
