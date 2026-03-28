using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorDominio.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ParserPipeline
{
    public class JsonToParser
    {
        private readonly IConverteJson<Dictionary<int, FuncaoDTO>> _conversor;
        private readonly IPathProvider _provider;
        private readonly ParserAst _parser;

        public JsonToParser(IConverteJson<Dictionary<int, FuncaoDTO>> conversor, IPathProvider provider)
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
            var codigosJson = _conversor.CarregarJson(caminhoPipeline);

            foreach(var elementos in codigosJson)
            {
                listaCodigos.Add(elementos.Value.NomeFuncao, elementos.Value.Codigo);
            }

            return listaCodigos;
        }
    }
}
