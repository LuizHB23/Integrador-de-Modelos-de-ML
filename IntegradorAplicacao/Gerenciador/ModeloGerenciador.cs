namespace InetradorAplicacao.Gerenciador
{
    public class ModeloGerenciador : IGerenciador
    {
        public string Salvar(string caminhoModelo)
        {
            // Pega o caminho da pasta
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataPath, "Integrador", "Modelos");

            // Cria a pasta se ela não existir
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            // Define o caminho final do arquivo
            string nomeArquivo = Path.GetFileName(caminhoModelo); // modelo.onnx
            string caminhoDestino = Path.Combine(appFolder, nomeArquivo);

            // Copia o arquivo (overwrite: true para atualizar se já existir)
            try
            {
                File.Copy(caminhoModelo, caminhoDestino, true);
                Console.WriteLine($"Modelo guardado com sucesso em: {caminhoDestino}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao mover modelo: {ex.Message}");
            }

            return caminhoDestino;
        }

        public void Carregar(string caminhoModelo)
        {
            throw new NotImplementedException();
        }
    }
}
