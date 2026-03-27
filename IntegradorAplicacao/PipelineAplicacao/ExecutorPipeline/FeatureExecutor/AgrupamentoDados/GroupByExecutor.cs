using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Interfaces;
using IntegradorDominio.FeatureEngineering.AgrupamentoDados;
using System.Linq.Expressions;


namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.AgrupamentoDados
{
    public class GroupByExecutor
    {
        private readonly GroupBy _model;

        public GroupByExecutor(GroupBy model)
        {
            _model = model;
        }
    }
}
