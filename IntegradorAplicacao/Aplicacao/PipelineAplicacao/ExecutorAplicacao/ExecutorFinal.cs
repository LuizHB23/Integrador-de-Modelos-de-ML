using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ParserPipeline;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.DTO.Interfaces;
using IntegradorAplicacao.Infraestrutura.ConversorJSON;
using IntegradorDominio.AST;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorAplicacao
{
    public class ExecutorFinal<T> where T : IPipelineExecutor
    {
        private readonly IConverteJson<Dictionary<int, T>> _conversor;
        private Dictionary<string, object?> _objetosUtilizados;
        private readonly Queue<BuilderExecutor> _executors;
        private ParserAst? _parser;

        public ExecutorFinal(IConverteJson<Dictionary<int, T>> conversor)
        {
            _objetosUtilizados = new();
            _executors = new Queue<BuilderExecutor>();
            _parser = new ParserAst();
            _conversor = conversor;
        }

        public DataFrame ExecutarTudo(DataFrame dataFrame)
        {
            _objetosUtilizados["df"] = dataFrame;

            while (_executors.Count > 0) 
            {
                var executor = _executors.Dequeue();
                dataFrame = executor.ExecutarMetodo(dataFrame);
                executor = null;
            }
            _objetosUtilizados.Clear();

            return dataFrame;
        }

        public void ConstroiSequenciaMetodoPipeline(string caminhoFuncao)
        {
            _objetosUtilizados["df"] = null;
            var listaMetodoPipeline = RecuperaMetodoPipeline(caminhoFuncao);

            foreach(var metodoPipeline in listaMetodoPipeline)
            {
                var builderExecutor = new BuilderExecutor(_objetosUtilizados);
                builderExecutor.ConstroiMetodo(metodoPipeline);
                _executors.Enqueue(builderExecutor);
            }

            _parser = null;
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
