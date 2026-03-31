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

            var resultado = new List<Single?>();

            for (int i = 0; i < n; i++)
            {
                resultado.Add(dataFrame.PegarColuna<Single?>(Operacao.left).Dados[i] + dataFrame.PegarColuna<Single?>(Operacao.right).Dados[i]);
            }

            dataFrame.AlterarColuna(Operacao.exit, resultado);

            return dataFrame;
        }
    }
}
