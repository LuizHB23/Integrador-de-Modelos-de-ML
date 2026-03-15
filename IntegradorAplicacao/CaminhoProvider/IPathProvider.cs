namespace IntegradorAplicacao.CaminhoProvider
{
    public interface IPathProvider
    {
        string GetCaminhoModelo();
        string GetCaminhoPipeline();
        string GetCaminhoSchema();
    }
}
