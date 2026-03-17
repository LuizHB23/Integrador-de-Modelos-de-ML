using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.DTO;
using System.Text.Json;

namespace IntegradorAplicacao.ConversorJson
{
    public class ModeloJson : IConverteJson<ModeloDTO>
    {
        private readonly IPathProvider _provider;

        public ModeloJson(IPathProvider provider)
        {
            _provider = provider;
        }

        public void ConverteJson(ModeloDTO modelo)
        {
            string caminhoJson = Path.Combine(_provider.GetCaminhoModelo(), modelo.Nome, "modelo.json");

            using (var sw = new StreamWriter(caminhoJson))
            {
                var texto = JsonSerializer.Serialize(modelo);
                sw.Write(texto);
            }
        }
        public List<ModeloDTO> CarregarJson(string caminho)
        {
            throw new NotImplementedException();
        }

        public void EscreverJson(ModeloDTO objeto)
        {
            throw new NotImplementedException();
        }
    }
}
