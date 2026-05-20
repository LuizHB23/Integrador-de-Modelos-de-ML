using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors
{
    public abstract class FeatureExecutorBase<T> : IExecutorBase where T : class 
    {
        public T Operacao { get; }
        public abstract object Executar(DataFrame dataFrame);

        protected FeatureExecutorBase(T operacao)
        {
            Operacao = operacao;
        }
    }
}
