using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesAritmeticas
{
    public class ModExecutor : FeatureExecutorBase<Mod>
    {
        public ModExecutor(Mod operacao) : base(operacao) { }

        public override DataFrame Executar(DataFrame dataFrame)
        {
            var n = dataFrame.QuantidadeLinhas;
            var divisor = Convert.ToInt32(Operacao.value);

            var resultado = new List<Single?>();

            for (int i = 0; i < n; i++)
            {
                resultado.Add(dataFrame.PegarColuna<Single?>(Operacao.col).Dados[i] % divisor);
            }

            dataFrame.AlterarColuna(Operacao.exit, resultado);

            return dataFrame;
        }
    }
}
