using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace IntegradorAplicacao.ConversorJson
{
    public class SchemaJson : IConverteJson<SchemaDTO>
    {
        private readonly IPathProvider _provider;
        private List<SchemaDTO> _listaSchema;
        private  string _caminhoJson;

        public SchemaJson(IPathProvider provider)
        {
            _provider = provider;
            _listaSchema = new List<SchemaDTO>();
            _caminhoJson = string.Empty;
        }

        public void ConverteJson(SchemaDTO schema)
        {
            _caminhoJson = Path.Combine(_provider.GetCaminhoModelo(), schema.NomeModelo, "schema.json");

            EscreverJson(schema);

            using (var sw = new StreamWriter(_caminhoJson))
            {
                var texto = JsonSerializer.Serialize(_listaSchema);
                sw.Write(texto);
            }
        }

        public List<SchemaDTO> CarregarJson(string caminho)
        {
            using (var sr = new StreamReader(caminho))
            {
                var texto = sr.ReadToEnd();
                var listaSchema = JsonSerializer.Deserialize<List<SchemaDTO>>(texto);

                return listaSchema!;
            }
        }

        public void EscreverJson(SchemaDTO schema)
        {

            if (File.Exists(_caminhoJson))
            {
                _listaSchema = CarregarJson(_caminhoJson);
            }

            _listaSchema.Add(schema);
        }
    }
}
