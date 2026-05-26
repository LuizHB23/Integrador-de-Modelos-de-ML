namespace IntegradorAplicacao.Infraestrutura.CaminhoProvider
{
    public interface IPathProvider
    {
        string GetCaminhoPastasMatriz();
        string GetCaminhoPastaModelo(string nomeModelo);
        string GetCaminhoAppConfig(string nomeModelo);
        string GetCaminhoModeloEmUsoConfig(string nomeModelo);
        string GetCaminhoModeloConfig(string nomeModelo);
        string GetCaminhoSchemaConfig(string nomeModelo);
        string GetCaminhoPipelineConfig(string nomeModelo);
        string GetCaminhoTransformadorConfig(string nomeModelo);
        string GetCaminhoSaidaConfig(string nomeModelo);
    }
}
