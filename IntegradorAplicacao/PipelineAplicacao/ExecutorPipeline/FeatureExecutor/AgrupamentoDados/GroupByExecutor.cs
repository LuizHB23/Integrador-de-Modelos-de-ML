using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.AgrupamentoDados;


namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.AgrupamentoDados
{
    public class GroupByExecutor : FeatureExecutorBase<GroupBy>
    {

        public GroupByExecutor(GroupBy operacao) : base(operacao) { }

        public override DataFrame Executar(DataFrame df)
        {
            throw new NotImplementedException();
        }
    }
}
