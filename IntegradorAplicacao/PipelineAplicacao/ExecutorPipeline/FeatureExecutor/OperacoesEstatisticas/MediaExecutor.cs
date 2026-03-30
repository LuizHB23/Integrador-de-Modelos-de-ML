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
            int quantidadeLinhas = dataFrame.QuantidadeLinhas;
            float? valor = 0.0f;
            var valorColuna = dataFrame.PegarColuna<Single?>(Operacao.col).Dados;

            for(int i = 0; i < quantidadeLinhas; i++)
            {
                if(valorColuna[i] is not null)
                {
                    valor += valorColuna[i];
                }
            }

            return valor / quantidadeLinhas;
        }
    }
}
