using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesAritmeticas
{
    public class SomarExecutor : FeatureExecutorBase<Somar>
    {
        public SomarExecutor(Somar operacao) : base(operacao) { }

        public override DataFrame Executar(DataFrame dataFrame)
        {
            var n = dataFrame.QuantidadeLinhas;

            var resultado = new List<float?>();

            for (int i = 0; i < n; i++)
            {
                resultado.Add(dataFrame.PegarColuna<float?>(Operacao.left).Dados[i] + dataFrame.PegarColuna<float?>(Operacao.right).Dados[i]);
            }

            dataFrame.AlteraColuna(Operacao.exit, resultado);

            return dataFrame;
        }
    }
}
