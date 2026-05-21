using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorDominio.AST;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ParserPipeline
{
    public class JsonToParser
    {
        private readonly ConversorJson _conversor;
        private readonly IPathProvider _provider;
        private readonly ParserAst _parser;

        public JsonToParser(ConversorJson conversor, IPathProvider provider)
        {
            _conversor = conversor;
            _provider = provider;
            _parser = new ParserAst();
        }

        public MetodoPipeline EnviaMetodoPipeline(string caminhoPipeline)
        {
            var metodoNomeCorpo = CarregarCodigos(caminhoPipeline);
            return _parser.Parse(metodoNomeCorpo);
        }

        private Dictionary<string, List<string>> CarregarCodigos(string caminhoPipeline)
        {
            var listaCodigos = new Dictionary<string, List<string>>();
            var codigosJson = _conversor.CarregarJson<Dictionary<int, FuncaoDTO>>(caminhoPipeline);

            foreach(var elementos in codigosJson)
            {
                listaCodigos.Add(elementos.Value.NomeFuncao, elementos.Value.Codigo);
            }

            return listaCodigos;
        }
    }
}
