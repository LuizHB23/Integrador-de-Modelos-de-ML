using IntegradorDominio.Models.Configuracao.Interfaces;
using System.Text.Json;

namespace IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson.ConverteJson
{
    public class ConfiguradoresJson<T> : IConverteJson<List<T>> where T : IListaConfiguracao
    {
        public async Task ConverteJsonAsync(List<T> objeto, string caminho)
        {
            string texto = string.Empty;

            if (objeto.Count > 0)
            {
                texto = JsonSerializer.Serialize(objeto);

                File.WriteAllText(caminho, texto);
            }

        }

        public async Task<List<T>> CarregarJsonAsync(string caminho)
        {
            if (File.Exists(caminho))
            {
                var texto = File.ReadAllText(caminho);

                if (!string.IsNullOrWhiteSpace(texto))
                {
                    return JsonSerializer.Deserialize<List<T>>(texto)
                   ?? new List<T>();
                }
            }

            return new List<T>();
        }
    }
}
