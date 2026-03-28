using IntegradorAplicacao.PipelineAplicacao.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.AgrupamentoDados;
using System.Linq.Expressions;


namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.AgrupamentoDados
{
    public class GroupByExecutor : FeatureExecutor<GroupBy>
    {

        public GroupByExecutor(GroupBy operacao) : base(operacao) { }

        public override DataFrame Executar(DataFrame df)
        {
            throw new NotImplementedException();
        }
    }
}
