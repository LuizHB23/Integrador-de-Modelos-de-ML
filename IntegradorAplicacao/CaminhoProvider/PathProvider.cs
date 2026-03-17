using System.IO;

namespace IntegradorAplicacao.CaminhoProvider
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
    }
}
