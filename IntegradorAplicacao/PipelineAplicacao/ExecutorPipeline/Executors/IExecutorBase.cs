using IntegradorDominio.DataFrameModel;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors
{
    public interface IExecutorBase
    {
        object Executar(DataFrame df);
    }
}
