using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using System.Text.Json;

namespace IntegradorAplicacao.Infraestrutura.ConversorJSON
{
    public class ModeloJson : IConverteJson<ModeloDTO>
    {
        private readonly IPathProvider _provider;

        public ModeloJson(IPathProvider provider)
        {
            _provider = provider;
        }

        public void ConverteJson(ModeloDTO modelo)
        {
            string caminhoJson = Path.Combine(_provider.GetCaminhoModelo(), modelo.NomeModelo, "modelo.json");
            var texto = JsonSerializer.Serialize(modelo);
            File.WriteAllText(caminhoJson, texto);
        }

        public ModeloDTO CarregarJson(string caminho)
        {
            if (File.Exists(caminho))
            {
                var texto = File.ReadAllText(caminho);

                if (!string.IsNullOrWhiteSpace(texto))
                {
                    return JsonSerializer.Deserialize<ModeloDTO>(texto)
                   ?? throw new Exception($"Arquivo corrompido: {caminho}");
                }
            }

            throw new Exception($"Pasta vazia: {caminho}");
        }
    }
}
