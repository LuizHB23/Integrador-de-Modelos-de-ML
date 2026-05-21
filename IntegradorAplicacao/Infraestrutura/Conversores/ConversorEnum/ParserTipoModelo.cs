using IntegradorDominio.Models.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntegradorAplicacao.Infraestrutura.Conversores.ConversorEnum
{
    public static class ParserTipoModelo
    {
        public static TipoModelo StringParaTipoModelo(string tipoModeloString)
        {
            TipoModelo tipoModeloEnum;

            switch (tipoModeloString.ToLower().Trim())
            {
                case "regressão":
                    tipoModeloEnum = TipoModelo.Regressao;
                    break;

                case "classificação":
                    tipoModeloEnum = TipoModelo.Classificao;
                    break;

                default:
                    throw new Exception($"Tipo de Modelo não existente: {tipoModeloString}");
            }

            return tipoModeloEnum;
        }

        public static string TipoModeloParaString(TipoModelo tipoModeloEnum)
        {
            string tipoModeloString;

            switch (tipoModeloEnum)
            {
                case TipoModelo.Regressao:
                    tipoModeloString = "regressão";
                    break;

                case TipoModelo.Classificao:
                    tipoModeloString = "classificação";
                    break;

                default:
                    throw new Exception($"Tipo de Modelo não registrado: {tipoModeloEnum}");
            }

            return tipoModeloString;
        }
    }

    public class ParserTipoModeloJsonConverter : JsonConverter<TipoModelo>
    {
        public override TipoModelo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string textoJson = reader.GetString();

                return ParserTipoModelo.StringParaTipoModelo(textoJson);
            }

            throw new JsonException($"Esperava uma string para TipoModelo, mas veio {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, TipoModelo value, JsonSerializerOptions options)
        {
            string textoParaSalvar = ParserTipoModelo.TipoModeloParaString(value);

            writer.WriteStringValue(textoParaSalvar);
        }
    }
}
