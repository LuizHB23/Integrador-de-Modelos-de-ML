using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.OperacoesEstatisticas;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEstatisticas
{
    public class ModaExecutor : FeatureExecutorBase<Moda>
    {
        public ModaExecutor(Moda operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColuna<Single?>(Operacao.col);

            if (coluna == null)
                throw new Exception($"Coluna '{Operacao.col}' não encontrada.");

            Span<Single?> span = coluna.PegarColunaSpan();

            int n = span.Length;

            // 🔥 mantém Dictionary (não tem jeito aqui sem mudar modelo)
            var frequencias = new Dictionary<Single, int>();

            for (int i = 0; i < n; i++)
            {
                var v = span[i];

                if (!v.HasValue)
                    continue;

                float value = v.Value;

                if (frequencias.TryGetValue(value, out int count))
                    frequencias[value] = count + 1;
                else
                    frequencias[value] = 1;
            }

            float? moda = null;
            int maxFreq = 0;

            foreach (var kv in frequencias)
            {
                if (kv.Value > maxFreq)
                {
                    maxFreq = kv.Value;
                    moda = kv.Key;
                }
            }

            return moda;
        }
    }
}
