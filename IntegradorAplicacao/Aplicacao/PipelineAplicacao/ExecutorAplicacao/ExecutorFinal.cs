using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ParserPipeline;
using IntegradorAplicacao.DTO.Interfaces;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorDominio.AST;
using IntegradorDominio.Models.Configuracao;
using IntegradorDominio.Models.Configuracao.Interfaces;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorAplicacao
{
    public class ExecutorFinal<T> where T : IPipelineConfiguracao
    {
        private Dictionary<string, object?> _objetosUtilizados;
        private readonly Queue<BuilderExecutor> _executors;
        private ParserAst? _parser;

        public ExecutorFinal()
        {
            _objetosUtilizados = new();
            _executors = new Queue<BuilderExecutor>();
            _parser = new ParserAst();
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

        public async Task ConstroiSequenciaMetodoPipeline(T codigo)
        {
            _objetosUtilizados["df"] = null;
            var listaMetodoPipeline = await RecuperaMetodoPipeline(codigo);

            foreach(var metodoPipeline in listaMetodoPipeline)
            {
                var builderExecutor = new BuilderExecutor(_objetosUtilizados);
                builderExecutor.ConstroiMetodo(metodoPipeline);
                _executors.Enqueue(builderExecutor);
            }

            _parser = null;
        }

        private async Task<List<MetodoPipeline>> RecuperaMetodoPipeline(T codigo)
        {
            var listaMetodoPipeline = new List<MetodoPipeline>();

            foreach (var elemento in codigo.Dicionario)
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
