using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace IntegradorAplicacao.ConversorJson
{
    public class PipelineJson : IConverteJson<Dictionary<int, FuncaoDTO>>
    {
        private readonly IPathProvider _provider;
        private string _caminhoJson;

        public PipelineJson(IPathProvider provider)
        {
            _provider = provider;
            _caminhoJson = string.Empty;
        }

        public void ConverteJson(Dictionary<int, FuncaoDTO> pipelineNovo)
        {
            string texto = string.Empty;

            if (pipelineNovo.Count != 0)
            {
                var card = pipelineNovo.First();

                _caminhoJson = Path.Combine(_provider.GetCaminhoModelo(), card.Value.NomeModelo, "pipeline.json");

                texto = JsonSerializer.Serialize(pipelineNovo);
            }

            File.WriteAllText(_caminhoJson, texto);
        }

        public Dictionary<int, FuncaoDTO> CarregarJson(string caminho)
        {
            if (File.Exists(caminho))
            {
                var texto = File.ReadAllText(caminho);

                if (!string.IsNullOrWhiteSpace(texto))
                {
                    return JsonSerializer.Deserialize<Dictionary<int, FuncaoDTO>>(texto)
                   ?? new Dictionary<int, FuncaoDTO>();
                }
            }

            return new Dictionary<int, FuncaoDTO>();
        }
    }
}
