using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesEstatisticas;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEstatisticas
{
    public class DesvioPadraoExecutor : FeatureExecutorBase<DesvioPadrao>
    {
        public DesvioPadraoExecutor(DesvioPadrao operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColuna<Single?>(Operacao.col);
            var span = coluna.PegarColunaSpan();

            double soma = 0;
            int count = 0;

            // 1ª passada: média
            for (int i = 0; i < span.Length; i++)
            {
                var v = span[i];
                if (v is null) continue;

                soma += v.Value;
                count++;
            }

            if (count == 0)
                return 0f;

            double media = soma / count;

            // 2ª passada: variância
            double somaQuadrada = 0;

            for (int i = 0; i < span.Length; i++)
            {
                var v = span[i];
                if (v is null) continue;

                double diff = v.Value - media;
                somaQuadrada += diff * diff;
            }

            double variancia = somaQuadrada / count;

            return (float)Math.Sqrt(variancia);
        }
    }
}
