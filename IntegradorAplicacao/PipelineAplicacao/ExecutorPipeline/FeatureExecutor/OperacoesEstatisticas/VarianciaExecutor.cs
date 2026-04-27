using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesEstatisticas;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEstatisticas
{
    public class VarianciaExecutor : FeatureExecutorBase<Variancia>
    {
        public VarianciaExecutor(Variancia operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColuna<Single?>(Operacao.col);

            if (coluna == null)
                throw new Exception($"Coluna '{Operacao.col}' não encontrada.");

            Span<Single?> span = coluna.PegarColunaSpan();

            int n = span.Length;

            float soma = 0;
            int count = 0;

            // 🔥 primeira passagem: média
            for (int i = 0; i < n; i++)
            {
                var v = span[i];
                if (v.HasValue)
                {
                    soma += v.Value;
                    count++;
                }
            }

            if (count == 0)
                return 0f;

            float media = soma / count;

            // 🔥 segunda passagem: variância
            float somaQuadrada = 0;

            for (int i = 0; i < n; i++)
            {
                var v = span[i];
                if (!v.HasValue)
                    continue;

                float diff = v.Value - media;
                somaQuadrada += diff * diff;
            }

            return somaQuadrada / count;
        }
    }
}
