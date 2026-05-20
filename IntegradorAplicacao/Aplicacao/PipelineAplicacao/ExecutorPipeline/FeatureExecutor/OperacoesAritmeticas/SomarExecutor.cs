using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesAritmeticas
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
            Span<float?> spanLeft = null;
            Span<float?> spanRight = null;

            if (Operacao.left is not null)
            {
                colLeft = dataFrame.PegarColuna<float?>(Operacao.left);
                spanLeft = colLeft.PegarColunaSpan();

            }

            if (Operacao.right is not null)
            {
                colRight = dataFrame.PegarColuna<float?>(Operacao.right);
                spanRight = colRight.PegarColunaSpan();
            }


            float valorConstante = Operacao.value != ""
                ? Convert.ToSingle(Operacao.value)
                : throw new Exception("Value precisa ter um valor numérico");

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
            else if (spanLeft != null && Operacao.value is not null)
            {
                for (int i = 0; i < n; i++)
                {
                    var a = spanLeft[i];

                    resultado[i] = a.HasValue
                        ? a.Value + valorConstante
                        : null;
                }
            }
            else if (spanRight != null && Operacao.value is not null)
            {
                for (int i = 0; i < n; i++)
                {
                    var b = spanRight[i];

                    resultado[i] = b.HasValue
                        ? valorConstante + b.Value
                        : null;
                }
            }
            else
            {
                throw new Exception("Precisa especificar ao menos uma coluna e/ou valor");
            }

            dataFrame.AlterarColuna(Operacao.exit, resultado);

            return dataFrame;
        }
    }
}
