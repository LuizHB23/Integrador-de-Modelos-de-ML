using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesAritmeticas
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

            float valorConstante = Operacao.value != ""
                ? Convert.ToSingle(Operacao.value)
                : throw new Exception("Value precisa ter um valor numérico");

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
                else if (hasLeft && Operacao.value is not null)
                {
                    var a = spanLeft[i];

                    resultado[i] = a.HasValue
                        ? a.Value * valorConstante
                        : null;
                }
                else if(hasRight && Operacao.value is not null)
                {
                    var b = spanRight[i];

                    resultado[i] = b.HasValue
                        ? valorConstante * b.Value
                        : null;
                }
                else
                {
                    throw new Exception("Precisa especificar ao menos uma coluna e/ou valor");
                }
            }

            dataFrame.AlterarColuna(Operacao.exit, resultado);

            return dataFrame;
        }
    }
}
