using IntegradorAplicacao.Interfaces;
using System.IO;

namespace IntegradorView.CaminhoProvider
{
    public class PathProvider : IPathProvider
    {
        private readonly string _appDataPath;

        public PathProvider()
        { 
            _appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _appDataPath = Path.Combine(_appDataPath, "Integrador", "Modelos");
        }

        public string GetCaminhoModelo() => Path.Combine(_appDataPath);

        public string GetCaminhoSchema() => Path.Combine(_appDataPath, "Schema");

        public string GetCaminhoPipeline() => Path.Combine(_appDataPath, "Pipeline");
    }
}
