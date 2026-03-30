using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.LimpezaDados;
using IntegradorDominio.FeatureEngineering.OperacoesEstatisticas;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEstatisticas
{
    public class MediaExecutor : FeatureExecutorBase<Media>
    {
        public MediaExecutor(Media operacao) : base(operacao) { }
        public override object Executar(DataFrame dataFrame)
        {
            int quantidadeLinhas = dataFrame.QuantidadeLinhas;
            float valor = 0;

            for(int i = 0; i < quantidadeLinhas; i++)
            {
                valor += dataFrame.PegarColuna<Single>(Operacao.col).Dados[i];
            }

            return valor / quantidadeLinhas;
        }
    }
}
