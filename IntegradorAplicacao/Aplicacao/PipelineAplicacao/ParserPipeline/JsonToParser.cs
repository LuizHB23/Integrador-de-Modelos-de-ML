using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorDominio.AST;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ParserPipeline
{
    public class JsonToParser
    {
        private readonly IConversorJson _conversor;
        private readonly IPathProvider _provider;
        private readonly ParserAst _parser;

        public JsonToParser(IConversorJson conversor, IPathProvider provider)
        {
            _conversor = conversor;
            _provider = provider;
            _parser = new ParserAst();
        }

        public async Task<MetodoPipeline> EnviaMetodoPipeline(string caminhoPipeline)
        {
            var metodoNomeCorpo = await CarregarCodigos(caminhoPipeline);
            return _parser.Parse(metodoNomeCorpo);
        }

        private async Task<Dictionary<string, List<string>>> CarregarCodigos(string caminhoPipeline)
        {
            var listaCodigos = new Dictionary<string, List<string>>();
            var codigosJson = await _conversor.CarregarJsonAsync<Dictionary<int, FuncaoDTO>>(caminhoPipeline);

            foreach(var elementos in codigosJson)
            {
                listaCodigos.Add(elementos.Value.NomeFuncao, elementos.Value.Codigo);
            }

            return listaCodigos;
        }
    }
}
