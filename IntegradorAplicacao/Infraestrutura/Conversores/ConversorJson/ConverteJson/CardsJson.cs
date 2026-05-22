using IntegradorAplicacao.DTO;
using IntegradorAplicacao.DTO.Interfaces;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using System.Text.Json;

namespace IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson.ConverteJson
{
    public class CardsJson<T> : IConverteJson<Dictionary<int, T>> where T : IItemNomeModelo
    {
        private readonly IPathProvider _provider;
        private string _caminhoJson;
        protected string _json;

        public CardsJson(IPathProvider provider)
        {
            _provider = provider;
            _caminhoJson = string.Empty;
            _json = PegaJson();
        }

        public async Task ConverteJsonAsync(Dictionary<int, T> objeto)
        {
            string texto = string.Empty;

            if (objeto.Count != 0)
            {
                var card = objeto.First();

                _caminhoJson = Path.Combine(_provider.GetCaminhoModelo(), card.Value.NomeModelo, _json);

                texto = JsonSerializer.Serialize(objeto);
            }

            File.WriteAllText(_caminhoJson, texto);
        }

        public async Task<Dictionary<int, T>> CarregarJsonAsync(string caminho)
        {
            if (File.Exists(caminho))
            {
                var texto = File.ReadAllText(caminho);

                if (!string.IsNullOrWhiteSpace(texto))
                {
                    return JsonSerializer.Deserialize<Dictionary<int, T>>(texto)
                   ?? new Dictionary<int, T>();
                }
            }

            return new Dictionary<int, T>();
        }

        private string PegaJson()
        {
            return typeof(T) switch
            {
                Type tipo when tipo == typeof(SchemaDTO) => "schema.json",
                Type tipo when tipo == typeof(FuncaoDTO) => "pipeline.json",
                Type tipo when tipo == typeof(TransformadorDTO) => "transformador.json",
                Type tipo when tipo == typeof(SaidaDTO) => "saida.json",
            };
        }
    }
}
