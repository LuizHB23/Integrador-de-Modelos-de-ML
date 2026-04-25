using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesAritmeticas
{
    public class MultiplicarExecutor : FeatureExecutorBase<Multiplicar>
    {
        public MultiplicarExecutor(Multiplicar operacao) : base(operacao) { }

        public override DataFrame Executar(DataFrame dataFrame)
        {
            var n = dataFrame.QuantidadeLinhas;

            var resultado = new float?[n];

            Span<float?> spanLeft = default;
            Span<float?> spanRight = default;

            bool hasLeft = Operacao.left is not null;
            bool hasRight = Operacao.right is not null;

            if (hasLeft)
                spanLeft = dataFrame.PegarColuna<float?>(Operacao.left!).PegarColunaSpan();

            if (hasRight)
                spanRight = dataFrame.PegarColuna<float?>(Operacao.right!).PegarColunaSpan();

            float valorConst = Convert.ToSingle(Operacao.value);

            for (int i = 0; i < n; i++)
            {
                if (hasLeft && hasRight)
                {
                    var a = spanLeft[i];
                    var b = spanRight[i];

                    resultado[i] = (a.HasValue && b.HasValue)
                        ? a.Value * b.Value
                        : null;
                }
                else if (hasLeft)
                {
                    var a = spanLeft[i];

                    resultado[i] = a.HasValue
                        ? a.Value * valorConst
                        : null;
                }
                else
                {
                    var b = spanRight[i];

                    resultado[i] = b.HasValue
                        ? valorConst * b.Value
                        : null;
                }
            }

            dataFrame.AlterarColuna(Operacao.exit, resultado);

            return dataFrame;
        }
    }
}
