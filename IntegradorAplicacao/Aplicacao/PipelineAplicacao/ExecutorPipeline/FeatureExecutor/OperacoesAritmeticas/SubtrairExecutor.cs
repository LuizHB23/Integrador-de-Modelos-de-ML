using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesAritmeticas
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

            // 🔥 constantes
            float valorConstNum = 0f;
            DateTime valorConstDate = default;

            if (hasLeft && hasRight)
            {
                if (isDateTime)
                {
                    spanLeftDate = dataFrame.PegarColuna<DateTime?>(Operacao.left!).PegarColunaSpan();
                    spanRightDate = dataFrame.PegarColuna<DateTime?>(Operacao.right!).PegarColunaSpan();
                }
                else
                {
                    spanLeftNum = dataFrame.PegarColuna<float?>(Operacao.left!).PegarColunaSpan();
                    spanRightNum = dataFrame.PegarColuna<float?>(Operacao.right!).PegarColunaSpan();
                }
            }
            else if (hasLeft && Operacao.value != "")
            {
                if (isDateTime)
                {
                    spanLeftDate = dataFrame.PegarColuna<DateTime?>(Operacao.left!).PegarColunaSpan();
                    valorConstDate = Convert.ToDateTime(Operacao.value);
                }
                else
                {
                    spanLeftNum = dataFrame.PegarColuna<float?>(Operacao.left!).PegarColunaSpan();
                    valorConstNum = Convert.ToSingle(Operacao.value);

                }
            }
            else if (hasRight && Operacao.value != "")
            {
                if (isDateTime)
                {
                    spanRightDate = dataFrame.PegarColuna<DateTime?>(Operacao.right!).PegarColunaSpan();
                    valorConstDate = Convert.ToDateTime(Operacao.value);
                }
                else
                {
                    spanRightNum = dataFrame.PegarColuna<float?>(Operacao.right!).PegarColunaSpan();
                    valorConstNum = Convert.ToSingle(Operacao.value);
                }
            }
            else
            {
                throw new Exception("Precisa especificar ao menos uma coluna e/ou valor");
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
