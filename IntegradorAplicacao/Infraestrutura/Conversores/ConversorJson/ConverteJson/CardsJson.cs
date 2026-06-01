using IntegradorAplicacao.DTO.Interfaces;
using System.Text.Json;

namespace IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson.ConverteJson
{
    public class CardsJson<T> : IConverteJson<Dictionary<int, T>> where T : IItemNomeModelo
    {
        public async Task<Dictionary<int, T>> CarregarJsonAsync(string caminho)
        {
            if (File.Exists(caminho))
            {
                var texto = File.ReadAllText(caminho);

                if (!string.IsNullOrWhiteSpace(texto))
                {
                    return JsonSerializer.Deserialize<Dictionary<int, T>>(texto)
                   ?? new Dictionary<int, T>();
                }
            }

            return new Dictionary<int, T>();
        }

        public async Task ConverteJsonAsync(Dictionary<int, T> objeto, string caminho)
        {
            throw new NotImplementedException();
        }

        
    }
}
