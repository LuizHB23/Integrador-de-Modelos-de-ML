using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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

        public void ConverteJson(Dictionary<int, SchemaDTO> schemas)
        {
            var (posicao, schema) = schemas.First();
            _caminhoJson = Path.Combine(_provider.GetCaminhoModelo(), schema.NomeModelo, "schema.json");
            string texto = JsonSerializer.Serialize(schemas);
            File.WriteAllText(_caminhoJson, texto);
        }

        public Dictionary<int, SchemaDTO> CarregarJson(string caminho)
        {
            if(File.Exists(caminho))
                using (var sr = new StreamReader(caminho))
                {
                    var texto = sr.ReadToEnd();
                    var listaSchema = JsonSerializer.Deserialize<Dictionary<int, SchemaDTO>>(texto);

                    return listaSchema!;
                }
            else
                return new Dictionary<int, SchemaDTO>();
        }

        public void EscreverJson(Dictionary<int, SchemaDTO> schema)
        {
        }
    }
}
