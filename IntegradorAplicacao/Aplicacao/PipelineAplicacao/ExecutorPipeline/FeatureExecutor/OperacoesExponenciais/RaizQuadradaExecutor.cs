using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.OperacoesExponenciais;
using IntegradorDominio.Models.DataFrameModel;
using System.Diagnostics;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesExponenciais
{
    public class RaizQuadradaExecutor : FeatureExecutorBase<RaizQuadrada>
    {
        public RaizQuadradaExecutor(RaizQuadrada operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColuna<Single?>(Operacao.col);
            var span = coluna.PegarColunaSpan();

            for (int i = 0; i < span.Length; i++)
            {
                var v = span[i];

                if (!span[i].HasValue)
                    continue;

                if (span[i].Value < 0)
                    continue;

                span[i] = MathF.Sqrt(span[i].Value);
                Debug.WriteLine(span[i].Value);
            }

            return dataFrame;
        }
    }
}
