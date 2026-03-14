using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Interfaces;

namespace IntegradorAplicacao.Gerenciador
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
            string appFolder = _provider.GetCaminhoModelo();
            appFolder = Path.Combine(appFolder, modelo.Nome);

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            string nomeArquivo = Path.GetFileName(modelo.CaminhoPasta);
            string caminhoDestino = Path.Combine(appFolder, nomeArquivo);

            try
            {
                File.Copy(modelo.CaminhoPasta, caminhoDestino, true);
                Console.WriteLine($"Modelo guardado com sucesso em: {caminhoDestino}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao mover modelo: {ex.Message}");
            }

            return caminhoDestino;
        }

        public void Carregar(ModeloDTO modelo)
        {
            throw new NotImplementedException();
        }
    }
}
