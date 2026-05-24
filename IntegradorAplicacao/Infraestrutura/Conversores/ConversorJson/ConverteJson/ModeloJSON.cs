using IntegradorAplicacao.Infraestrutura.Conversores.ConversorEnum;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorDominio.Models.Configuracao;
using System.Text.Json;

namespace IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson.ConverteJson
{
    public class ModeloJson : IConverteJson<ModeloConfiguracao>
    {
        private readonly IPathProvider _provider;

        public ModeloJson(IPathProvider provider)
        {
            _provider = provider;
        }

        public async Task ConverteJsonAsync(ModeloConfiguracao modelo)
        {
            string caminhoJson = _provider.GetCaminhoModeloConfig(modelo.NomeModelo);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            options.Converters.Add(
                new ParserTipoModeloJsonConverter());

            var texto = JsonSerializer.Serialize(modelo, options);
            File.WriteAllText(caminhoJson, texto);
        }

        public async Task<ModeloConfiguracao> CarregarJsonAsync(string caminho)
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

                    return JsonSerializer.Deserialize<ModeloConfiguracao>(texto, options)
                   ?? throw new Exception($"Arquivo corrompido: {caminho}");
                }
            }

            throw new Exception($"Pasta vazia: {caminho}");
        }
    }
}
