using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;
using System;
using System.Collections.Generic;
using System.Text;

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

            var resultado = new List<object?>();

            // 🔥 resolve tipos
            var tipoLeft = hasLeft ? dataFrame.PegarColunaBase(Operacao.left).TipoDado : Operacao.value?.GetType();
            var tipoRight = hasRight ? dataFrame.PegarColunaBase(Operacao.right).TipoDado : Operacao.value?.GetType();

            bool isDateTime = (tipoLeft == typeof(DateTime) || tipoRight == typeof(DateTime)) || (tipoLeft == typeof(DateTime?) || tipoRight == typeof(DateTime?));

            // 🔥 colunas (se existirem)
            var colLeftNum = hasLeft ? dataFrame.PegarColuna<Single?>(Operacao.left) : null;
            var colRightNum = hasRight ? dataFrame.PegarColuna<Single?>(Operacao.right) : null;

            var colLeftDate = hasLeft ? dataFrame.PegarColuna<DateTime?>(Operacao.left) : null;
            var colRightDate = hasRight ? dataFrame.PegarColuna<DateTime?>(Operacao.right) : null;

            for (int i = 0; i < n; i++)
            {
                object? left = hasLeft
                    ? (isDateTime ? colLeftDate!.Dados[i] : colLeftNum!.Dados[i])
                    : (isDateTime ? Convert.ToDateTime(Operacao.value) : Convert.ToSingle(Operacao.value));

                object? right = hasRight
                    ? (isDateTime ? colRightDate!.Dados[i] : colRightNum!.Dados[i])
                    : (isDateTime ? Convert.ToDateTime(Operacao.value) : Convert.ToSingle(Operacao.value));

                if (left == null || right == null)
                {
                    resultado.Add(null);
                    continue;
                }

                if (isDateTime)
                {
                    var diff = ((DateTime)left - (DateTime)right).TotalDays;
                    resultado.Add((Single?)diff);
                }
                else
                {
                    resultado.Add((Single?)left - (Single?)right);
                }
            }

            // 🔥 define tipo final
            var tipoFinal = isDateTime ? typeof(Single?) : typeof(Single?);

            dataFrame.AlterarColuna(Operacao.exit, resultado, tipoFinal);

            return dataFrame;
        }
    }
}
