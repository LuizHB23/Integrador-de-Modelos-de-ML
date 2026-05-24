using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorDominio;

namespace IntegradorAplicacao.Infraestrutura.Gerenciador
{
    public class ModeloGerenciador : IGerenciador<ModeloDTO>
    {
        private readonly IPathProvider _provider;

        public ModeloGerenciador(IPathProvider provider)
        {
            _provider = provider;
        }

        public string Salvar(ModeloDTO modelo)
        {
            string appFolder = _provider.GetCaminhoPastaModelo(modelo.NomeModelo);
            Directory.CreateDirectory(appFolder);

            Directory.CreateDirectory(_provider.GetCaminhoAppConfig(modelo.NomeModelo));

            string nomeArquivo = Path.GetFileName(modelo.CaminhoPasta);
            string caminhoDestino = Path.Combine(appFolder, nomeArquivo);

            File.Copy(modelo.CaminhoPasta, caminhoDestino, true);

            return caminhoDestino;
        }

        public void Carregar(ModeloDTO modelo)
        {
            throw new NotImplementedException();
        }
    }
}
