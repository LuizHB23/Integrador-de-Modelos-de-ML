using IntegradorDominio.DataFrameModel;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors
{
    public interface IExecutorBase
    {
        DataFrame Executar(DataFrame df);
    }
}
