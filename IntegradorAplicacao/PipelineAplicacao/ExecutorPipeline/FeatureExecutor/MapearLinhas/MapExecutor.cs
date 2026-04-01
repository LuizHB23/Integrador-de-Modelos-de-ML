using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.Parser;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.MapearLinhas;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas
{
    public class MapExecutor : FeatureExecutorBase<Map>
    {
        private readonly ParserMap _parserMap;
        private Dictionary<string, object> _caseLinhas;

        public MapExecutor(Map operacao) : base(operacao) 
        {
            _parserMap = new();
            _caseLinhas = new();
        }

        public override object Executar(DataFrame dataFrame)
        {
            throw new NotImplementedException();
        }
    }
}
