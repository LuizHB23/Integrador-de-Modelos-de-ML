using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesAritmeticas
{
    public class SomarExecutor : FeatureExecutorBase<Somar>
    {
        public SomarExecutor(Somar operacao) : base(operacao) { }

        public override DataFrame Executar(DataFrame dataFrame)
        {
            int n = dataFrame.QuantidadeLinhas;

            var resultado = new float?[n];

            Coluna<float?>? colLeft = null;
            Coluna<float?>? colRight = null;

            if (Operacao.left is not null)
                colLeft = dataFrame.PegarColuna<float?>(Operacao.left);

            if (Operacao.right is not null)
                colRight = dataFrame.PegarColuna<float?>(Operacao.right);

            var spanLeft = colLeft.PegarColunaSpan();
            var spanRight = colRight.PegarColunaSpan();

            float valorConstante = Operacao.value != null
                ? Convert.ToSingle(Operacao.value)
                : 0f;

            if (spanLeft != null && spanRight != null)
            {
                for (int i = 0; i < n; i++)
                {
                    var a = spanLeft[i];
                    var b = spanRight[i];

                    resultado[i] = (a.HasValue && b.HasValue)
                        ? a.Value + b.Value
                        : null;
                }
            }
            else if (spanLeft != null)
            {
                for (int i = 0; i < n; i++)
                {
                    var a = spanLeft[i];

                    resultado[i] = a.HasValue
                        ? a.Value + valorConstante
                        : null;
                }
            }
            else if (spanRight != null)
            {
                for (int i = 0; i < n; i++)
                {
                    var b = spanRight[i];

                    resultado[i] = b.HasValue
                        ? valorConstante + b.Value
                        : null;
                }
            }

            dataFrame.AlterarColuna(Operacao.exit, resultado);

            return dataFrame;
        }
    }
}
