using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.LimpezaDados;
using IntegradorDominio.FeatureEngineering.OperacoesEstatisticas;
using System.Diagnostics;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEstatisticas
{
    public class MediaExecutor : FeatureExecutorBase<Media>
    {
        public MediaExecutor(Media operacao) : base(operacao) { }
        public override object Executar(DataFrame dataFrame)
        {
            var valorColuna = dataFrame.PegarColuna<Single?>(Operacao.col).Dados;

            float soma = 0f;
            int count = 0;

            foreach (var v in valorColuna)
            {
                if (v is not null)
                {
                    soma += v.Value;
                    count++;
                }
            }

            return count > 0 ? soma / count : 0f;
        }
    }
}
