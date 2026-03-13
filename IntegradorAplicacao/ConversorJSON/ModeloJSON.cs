using InetradorAplicacao.DTO;
using IntegradorAplicacao.Interfaces;
using System.Text.Json;

namespace IntegradorAplicacao.ConversorJSON
{
    public class ModeloJSON : IConverteJSON<ModeloDTO>
    {
        private readonly IPathProvider _provider;

        public ModeloJSON(IPathProvider provider)
        {
            _provider = provider;
        }

        public void ConverteJSON(ModeloDTO modelo)
        {
            string caminhoJson = Path.Combine(_provider.GetCaminhoModelo(), modelo.Nome, "modelo.json");

            using (var sw = new StreamWriter(caminhoJson))
            {
                var texto = JsonSerializer.Serialize(modelo);
                sw.Write(texto);
            }
        }
    }
}
