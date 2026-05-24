namespace IntegradorAplicacao.Infraestrutura.CaminhoProvider
{
    public class PathProvider : IPathProvider
    {
        private const string SubPastaRaiz = "Integrador";
        private const string PastaModelos = "Modelos";
        private const string PastaConfig = "config";

        private readonly string _appDataPath;

        public PathProvider()
        { 
            _appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _appDataPath = Path.Combine(_appDataPath, SubPastaRaiz, PastaModelos);
        }

        public string GetCaminhoPastasMatriz() => _appDataPath;
        public string GetCaminhoPastaModelo(string nomeModelo) => Path.Combine(_appDataPath, nomeModelo);
        public string GetCaminhoAppConfig(string nomeModelo) => Path.Combine(GetCaminhoPastaModelo(nomeModelo), PastaConfig);
        public string GetCaminhoModeloConfig(string nomeModelo) => Path.Combine(GetCaminhoAppConfig(nomeModelo), "modelo.json");
        public string GetCaminhoSchemaConfig(string nomeModelo) => Path.Combine(GetCaminhoAppConfig(nomeModelo), "schema.json");
        public string GetCaminhoPipelineConfig(string nomeModelo) => Path.Combine(GetCaminhoAppConfig(nomeModelo), "pipeline.json");
        public string GetCaminhoTransformadorConfig(string nomeModelo) => Path.Combine(GetCaminhoAppConfig(nomeModelo), "transformador.json");
        public string GetCaminhoSaidaConfig(string nomeModelo) => Path.Combine(GetCaminhoAppConfig(nomeModelo), "saida.json");
    }
}
