using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.OperacoesEspeciais;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEspeciais
{
    public class CeilExecutor : FeatureExecutorBase<Ceil>
    {
        public CeilExecutor(Ceil operacao) : base(operacao) { }

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
                    span[i] = (Single)MathF.Ceiling(valor.Value);
            }

            return dataFrame;
        }
    }
}
