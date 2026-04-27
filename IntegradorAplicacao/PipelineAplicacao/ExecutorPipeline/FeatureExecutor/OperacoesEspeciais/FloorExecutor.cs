using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesEspeciais;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEspeciais
{
    public class FloorExecutor : FeatureExecutorBase<Floor>
    {
        public FloorExecutor(Floor operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColuna<Single?>(Operacao.col);

            if (coluna == null)
                throw new Exception($"Coluna '{Operacao.col}' não encontrada.");

            Span<Single?> span = coluna.PegarColunaSpan();

            for (int i = 0; i < span.Length; i++)
            {
                var valor = span[i];

                if (valor.HasValue)
                    span[i] = (Single)MathF.Floor(valor.Value);
            }

            return dataFrame;
        }
    }
}
