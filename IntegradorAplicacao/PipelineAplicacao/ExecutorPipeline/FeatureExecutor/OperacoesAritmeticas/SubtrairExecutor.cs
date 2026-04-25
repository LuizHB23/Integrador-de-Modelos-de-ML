using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesAritmeticas
{
    public class SubtrairExecutor : FeatureExecutorBase<Subtrair>
    {
        public SubtrairExecutor(Subtrair operacao) : base(operacao) { }

        public override DataFrame Executar(DataFrame dataFrame)
        {
            var n = dataFrame.QuantidadeLinhas;

            bool hasLeft = Operacao.left != null;
            bool hasRight = Operacao.right != null;

            // 🔥 detectar tipo
            var tipoLeft = hasLeft ? dataFrame.PegarColunaBase(Operacao.left!).TipoDado : Operacao.value?.GetType();
            var tipoRight = hasRight ? dataFrame.PegarColunaBase(Operacao.right!).TipoDado : Operacao.value?.GetType();

            bool isDateTime =
                tipoLeft == typeof(DateTime) || tipoLeft == typeof(DateTime?) ||
                tipoRight == typeof(DateTime) || tipoRight == typeof(DateTime?);

            var resultado = new float?[n];

            // 🔥 spans numéricos
            Span<float?> spanLeftNum = default;
            Span<float?> spanRightNum = default;

            // 🔥 spans datetime
            Span<DateTime?> spanLeftDate = default;
            Span<DateTime?> spanRightDate = default;

            if (hasLeft)
            {
                if (isDateTime)
                    spanLeftDate = dataFrame.PegarColuna<DateTime?>(Operacao.left!).PegarColunaSpan();
                else
                    spanLeftNum = dataFrame.PegarColuna<float?>(Operacao.left!).PegarColunaSpan();
            }

            if (hasRight)
            {
                if (isDateTime)
                    spanRightDate = dataFrame.PegarColuna<DateTime?>(Operacao.right!).PegarColunaSpan();
                else
                    spanRightNum = dataFrame.PegarColuna<float?>(Operacao.right!).PegarColunaSpan();
            }

            // 🔥 constantes
            float valorConstNum = 0f;
            DateTime valorConstDate = default;

            if (!hasLeft || !hasRight)
            {
                if (isDateTime)
                    valorConstDate = Convert.ToDateTime(Operacao.value);
                else
                    valorConstNum = Convert.ToSingle(Operacao.value);
            }

            // 🔥 loop
            for (int i = 0; i < n; i++)
            {
                if (isDateTime)
                {
                    DateTime? left = hasLeft ? spanLeftDate[i] : valorConstDate;
                    DateTime? right = hasRight ? spanRightDate[i] : valorConstDate;

                    if (!left.HasValue || !right.HasValue)
                    {
                        resultado[i] = null;
                        continue;
                    }

                    var diff = (left.Value - right.Value).TotalDays;
                    resultado[i] = (float)diff;
                }
                else
                {
                    float? left = hasLeft ? spanLeftNum[i] : valorConstNum;
                    float? right = hasRight ? spanRightNum[i] : valorConstNum;

                    if (!left.HasValue || !right.HasValue)
                    {
                        resultado[i] = null;
                        continue;
                    }

                    resultado[i] = left.Value - right.Value;
                }
            }

            // 🔥 sempre float? (inclusive datetime vira dias)
            dataFrame.AlterarColuna(Operacao.exit, resultado);

            return dataFrame;
        }
    }
}
