using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.LimpezaDados;
using IntegradorDominio.FeatureEngineering.OperacoesEstatisticas;
using IntegradorDominio.Models.DataFrameModel;
using System.Diagnostics;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEstatisticas
{
    public class MediaExecutor : FeatureExecutorBase<Media>
    {
        public MediaExecutor(Media operacao) : base(operacao) { }
        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColuna<Single?>(Operacao.col);

            if (coluna == null)
                throw new Exception($"Coluna '{Operacao.col}' não encontrada ou tipo inválido.");

            var span = coluna.PegarColunaSpan();

            float soma = 0f;
            int count = 0;

            for (int i = 0; i < span.Length; i++)
            {
                var v = span[i];

                if (v.HasValue)
                {
                    soma += v.Value;
                    count++;
                }
            }

            return count > 0 ? soma / count : 0f;
        }
    }
}
