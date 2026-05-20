using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorDominio.Models.Configuracao;
using System.Text.Json;

namespace IntegradorAplicacao.Infraestrutura.ConversorJson.Conversores
{
    public class ModeloJson : IConverteJson<ModeloConfiguracao>
    {
        private readonly IPathProvider _provider;

        public ModeloJson(IPathProvider provider)
        {
            _provider = provider;
        }

        public void ConverteJson(ModeloConfiguracao modelo)
        {
            string caminhoJson = Path.Combine(_provider.GetCaminhoModelo(), modelo.NomeModelo, "modelo.json");
            var texto = JsonSerializer.Serialize(modelo);
            File.WriteAllText(caminhoJson, texto);
        }

        public ModeloConfiguracao CarregarJson(string caminho)
        {
            if (File.Exists(caminho))
            {
                var texto = File.ReadAllText(caminho);

                if (!string.IsNullOrWhiteSpace(texto))
                {
                    return JsonSerializer.Deserialize<ModeloConfiguracao>(texto)
                   ?? throw new Exception($"Arquivo corrompido: {caminho}");
                }
            }

            throw new Exception($"Pasta vazia: {caminho}");
        }
    }
}
