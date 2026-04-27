using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesExponenciais;
using System.Diagnostics;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesExponenciais
{
    public class ExponencialExecutor : FeatureExecutorBase<Exponencial>
    {
        public ExponencialExecutor(Exponencial operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColuna<Single?>(Operacao.col);

            var span = coluna.PegarColunaSpan();
            int n = span.Length;

            for (int i = 0; i < n; i++)
            {
                var v = span[i];

                if (v.HasValue)
                    span[i] = (float)Math.Exp(v.Value);
                else
                    span[i] = null;
            }

            return dataFrame;
        }
    }
}
