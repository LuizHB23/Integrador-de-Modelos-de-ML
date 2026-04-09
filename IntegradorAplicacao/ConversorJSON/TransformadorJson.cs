using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace IntegradorAplicacao.ConversorJson
{
    public class TransformadorJson : IConverteJson<Dictionary<int, TransformadorDTO>>
    {
        private readonly IPathProvider _provider;
        private string _caminhoJson;

        public TransformadorJson(IPathProvider provider)
        {
            _provider = provider;
            _caminhoJson = string.Empty;
        }
        public void ConverteJson(Dictionary<int, TransformadorDTO> transformadorNovo)
        {
            string texto = string.Empty;

            if (transformadorNovo.Count != 0)
            {
                var card = transformadorNovo.First();

                _caminhoJson = Path.Combine(_provider.GetCaminhoModelo(), card.Value.NomeModelo, "transformador.json");

                texto = JsonSerializer.Serialize(transformadorNovo);
            }

            File.WriteAllText(_caminhoJson, texto);
        }

        public Dictionary<int, TransformadorDTO> CarregarJson(string caminho)
        {
            if (File.Exists(caminho))
            {
                var texto = File.ReadAllText(caminho);

                if (!string.IsNullOrWhiteSpace(texto))
                {
                    return JsonSerializer.Deserialize<Dictionary<int, TransformadorDTO>>(texto)
                   ?? new Dictionary<int, TransformadorDTO>();
                }
            }

            return new Dictionary<int, TransformadorDTO>();
        }

    }
}
