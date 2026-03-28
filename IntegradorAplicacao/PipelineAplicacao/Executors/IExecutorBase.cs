using IntegradorDominio.DataFrameModel;

namespace IntegradorAplicacao.PipelineAplicacao.Interfaces
{
    public interface IExecutorBase
    {
        DataFrame Executar(DataFrame df);
    }
}
