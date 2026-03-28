using IntegradorAplicacao.PipelineAplicacao.Interfaces;
using IntegradorDominio.DataFrameModel;

namespace IntegradorAplicacao.PipelineAplicacao.Executors
{
    public abstract class FeatureExecutor<T> : IExecutorBase where T : class 
    {
        T Operacao { get; }
        public abstract DataFrame Executar(DataFrame df);

        protected FeatureExecutor(T operacao)
        {
            Operacao = operacao;
        }
    }
}
