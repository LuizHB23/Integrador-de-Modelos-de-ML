using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.DTO;
using System.Text.Json;

namespace IntegradorAplicacao.ConversorJson.CardsJson
{
    public class SchemaJson : CardsJson<SchemaDTO>
    {
        public SchemaJson(IPathProvider provider) : base(provider)
        {
            _json = "schema.json";
        }
    }
}
