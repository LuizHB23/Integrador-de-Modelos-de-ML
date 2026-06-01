namespace IntegradorDominio.Models.Configuracao.Interfaces
{
    public interface ICardsConfiguracao<T> where T: class
    {
        string NomeModelo { get; set; }
        string Versao { get; set; }
        Dictionary<int, T> Dicionario { get; set; }
    }
}
