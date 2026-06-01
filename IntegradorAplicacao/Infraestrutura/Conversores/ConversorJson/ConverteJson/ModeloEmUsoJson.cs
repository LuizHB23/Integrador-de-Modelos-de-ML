using IntegradorAplicacao.Infraestrutura.Conversores.ConversorEnum;
using IntegradorDominio.Models.Configuracao;
using System.Text.Json;

namespace IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson.ConverteJson
{
    public class ModeloEmUsoJson : IConverteJson<ModeloEmUsoConfiguracao>
    {
        public async Task ConverteJsonAsync(ModeloEmUsoConfiguracao modelo, string caminho)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            options.Converters.Add(
                new ParserTipoModeloJsonConverter());

            var texto = JsonSerializer.Serialize(modelo, options);
            File.WriteAllText(caminho, texto);
        }

        public async Task<ModeloEmUsoConfiguracao> CarregarJsonAsync(string caminho)
        {
            if (File.Exists(caminho))
            {
                var texto = File.ReadAllText(caminho);

                if (!string.IsNullOrWhiteSpace(texto))
                {
                    var options = new JsonSerializerOptions
                    {
                        Converters =
                            {
                                new ParserTipoModeloJsonConverter()
                            }
                    };

                    return JsonSerializer.Deserialize<ModeloEmUsoConfiguracao>(texto, options)
                   ?? throw new Exception($"Arquivo corrompido: {caminho}");
                }
            }

            throw new Exception($"Pasta vazia: {caminho}");
        }
    }
}
