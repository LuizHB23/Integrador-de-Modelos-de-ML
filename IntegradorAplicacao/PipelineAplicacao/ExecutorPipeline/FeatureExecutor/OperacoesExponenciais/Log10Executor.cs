using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesExponenciais;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesExponenciais
{
    public class Log10Executor : FeatureExecutorBase<Log10>
    {
        public Log10Executor(Log10 operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColuna<Single?>(Operacao.col);

            var span = coluna.PegarColunaSpan();
            int n = span.Length;

            for (int i = 0; i < n; i++)
            {
                var v = span[i];

                if (v.HasValue && v.Value > 0)
                    span[i] = (float)Math.Log10(v.Value);
                else
                    span[i] = v;
            }

            return dataFrame;
        }
    }
}
