using IntegradorDominio.Models.ModeloEtapas;
namespace IntegradorDominio.Models.Configuracao.Interfaces
{
    public interface IPipelineConfiguracao
    {
        string Versao {  get; set; }
        public Dictionary<int, Pipeline> Dicionario { get; set; }
    }
}
