using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.ConversorJSON;
using System.Text.Json;

namespace IntegradorAplicacao.Infraestrutura.ConversorJSON.CardsJson
{
    public class SchemaJson : CardsJson<SchemaDTO>
    {
        public SchemaJson(IPathProvider provider) : base(provider)
        {
            _json = "schema.json";
        }
    }
}
