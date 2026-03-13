namespace IntegradorAplicacao.Interfaces
{
    public interface IPathProvider
    {
        string GetCaminhoModelo();
        string GetCaminhoPipeline();
        string GetCaminhoSchema();
    }
}
