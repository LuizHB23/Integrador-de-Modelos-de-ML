using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.DTO;
using System.Text.Json;

namespace IntegradorAplicacao.ConversorJson
{
    public class SchemaJson : IConverteJson<Dictionary<int, SchemaDTO>>
    {
        private readonly IPathProvider _provider;
        private  string _caminhoJson;

        public SchemaJson(IPathProvider provider)
        {
            _provider = provider;
            _caminhoJson = string.Empty;
        }

        public void ConverteJson(Dictionary<int, SchemaDTO> schemaNovo)
        {
            string texto = string.Empty;

            if (schemaNovo.Count != 0)
            {
                var card = schemaNovo.First();

                _caminhoJson = Path.Combine(_provider.GetCaminhoModelo(), card.Value.NomeModelo, "schema.json");

                texto = JsonSerializer.Serialize(schemaNovo);
            }

            File.WriteAllText(_caminhoJson, texto);
        }

        public Dictionary<int, SchemaDTO> CarregarJson(string caminho)
        {
            if (File.Exists(caminho))
            {
                var texto = File.ReadAllText(caminho);

                if(!string.IsNullOrWhiteSpace(texto))
                {
                    return JsonSerializer.Deserialize<Dictionary<int, SchemaDTO>>(texto)
                   ?? new Dictionary<int, SchemaDTO>();
                }
            }

            return new Dictionary<int, SchemaDTO>();
        }
    }
}
