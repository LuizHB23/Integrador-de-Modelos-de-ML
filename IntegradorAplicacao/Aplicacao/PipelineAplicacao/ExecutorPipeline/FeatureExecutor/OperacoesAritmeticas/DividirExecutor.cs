using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesAritmeticas
{
    public class DividirExecutor : FeatureExecutorBase<Dividir>
    {
        public DividirExecutor(Dividir operacao) : base(operacao) { }

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

                    resultado[i] = (a.HasValue && b.HasValue && b.Value != 0f)
                        ? a.Value / b.Value
                        : null;
                }
                else if (hasLeft && Operacao.value != "")
                {
                    var a = spanLeft[i];

                    resultado[i] = (a.HasValue && valorConstante != 0f)
                        ? a.Value / valorConstante
                        : null;
                }
                else if (hasRight && Operacao.value != "")
                {
                    var b = spanRight[i];

                    resultado[i] = (b.HasValue && b.Value != 0f)
                        ? valorConstante / b.Value
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
