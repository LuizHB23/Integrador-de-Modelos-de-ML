using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.OperacoesEstatisticas;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEstatisticas
{
    public class MedianaExecutor : FeatureExecutorBase<Mediana>
    {
        public MedianaExecutor(Mediana operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColuna<Single?>(Operacao.col);

            if (coluna == null)
                throw new Exception($"Coluna '{Operacao.col}' não encontrada.");

            Span<Single?> span = coluna.PegarColunaSpan();

            int n = span.Length;

            // buffer direto (sem List intermediária)
            float[] buffer = new float[n];
            int count = 0;

            for (int i = 0; i < n; i++)
            {
                var v = span[i];
                if (v.HasValue)
                    buffer[count++] = v.Value;
            }

            if (count == 0)
                return 0f;

            // ordena só o que importa
            Array.Sort(buffer, 0, count);

            int mid = count / 2;

            float mediana;

            if (count % 2 == 0)
            {
                mediana = (buffer[mid - 1] + buffer[mid]) / 2f;
            }
            else
            {
                mediana = buffer[mid];
            }

            return mediana;
        }
    }
}
