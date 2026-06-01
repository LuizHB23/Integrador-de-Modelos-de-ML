using System.Text.Json;

namespace IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson.ConverteJson
{
    public class TempJson<T> : IConverteJson<T> where T : class
    {
        public async Task<T> CarregarJsonAsync(string caminho)
        {
            if (File.Exists(caminho))
            {
                var texto = File.ReadAllText(caminho);

                if (!string.IsNullOrWhiteSpace(texto))
                {
                    return JsonSerializer.Deserialize<T>(texto)
                   ?? throw new Exception("Erro ao gerar o objeto");
                }
            }

            throw new Exception("Nenhum arquivo temporário guardado");
        }

        public async Task ConverteJsonAsync(T objeto, string caminho)
        {
            string texto = string.Empty;

            if (objeto is not null)
            {
                texto = JsonSerializer.Serialize(objeto);

                File.WriteAllText(caminho, texto);
            }
        }
    }
}
