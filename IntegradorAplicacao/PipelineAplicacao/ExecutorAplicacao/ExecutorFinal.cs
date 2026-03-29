using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.PipelineAplicacao.ParserPipeline;
using IntegradorDominio.AST;
using IntegradorDominio.DataFrameModel;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorAplicacao
{
    public class ExecutorFinal
    {
        private readonly IConverteJson<Dictionary<int, FuncaoDTO>> _conversor;
        private readonly List<BuilderExecutor> _executors;
        private readonly ParserAst _parser;

        public ExecutorFinal(IConverteJson<Dictionary<int, FuncaoDTO>> conversor)
        {
            _executors = new List<BuilderExecutor>();
            _parser = new ParserAst();
            _conversor = conversor;
        }

        public DataFrame ExecutarTudo(DataFrame dataFrame)
        {
            foreach (var executor in _executors) 
            {
                dataFrame = executor.ExecutarMetodo(dataFrame);
            }
            _executors.Clear();

            return dataFrame;
        }

        public void ConstroiSequenciaMetodoPipeline(string caminhoFuncao)
        {
            var listaMetodoPipeline = RecuperaMetodoPipeline(caminhoFuncao);

            foreach(var metodoPipeline in listaMetodoPipeline)
            {
                var builderExecutor = new BuilderExecutor();
                builderExecutor.ConstroiMetodo(metodoPipeline);
                _executors.Add(builderExecutor);
            }
        }

        private List<MetodoPipeline> RecuperaMetodoPipeline(string caminhoFuncao)
        {
            var codigosJson = _conversor.CarregarJson(caminhoFuncao);
            var listaMetodoPipeline = new List<MetodoPipeline>();

            foreach (var elemento in codigosJson)
            {
                var metodoNomeCorpo = new Dictionary<string, List<string>>
                    {
                        { elemento.Value.NomeFuncao, elemento.Value.Codigo }
                    };

                listaMetodoPipeline.Add(_parser.Parse(metodoNomeCorpo));
            }

            return listaMetodoPipeline;
        }
    }
}
