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

            var resultado = new float[n];

            for (int i = 0; i < n; i++)
            {
                //resultado[i] = Operacao.Coluna.Dados[i] % Operacao.Divisor;
            }

            //dataFrame.AddColumn(Operacao.NomeColunaSaida, resultado);

            return dataFrame;
        }
    }
}
