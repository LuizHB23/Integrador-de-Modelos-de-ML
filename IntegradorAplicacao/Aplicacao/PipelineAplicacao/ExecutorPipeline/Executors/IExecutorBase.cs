using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors
{
    public interface IExecutorBase
    {
        object Executar(DataFrame df);
    }
}
