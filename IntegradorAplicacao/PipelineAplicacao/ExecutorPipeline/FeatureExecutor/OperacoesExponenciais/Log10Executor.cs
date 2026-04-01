using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesExponenciais;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesExponenciais
{
    public class Log10Executor : FeatureExecutorBase<Log10>
    {
        public Log10Executor(Log10 operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColuna<Single?>(Operacao.col);
            int quantidadeLinhas = dataFrame.QuantidadeLinhas;
            var resultado = new List<Single?>();
            Single? valor;
            double valorDouble;

            for (int i = 0; i < quantidadeLinhas; i++)
            {
                valor = (Single?)coluna.PegarValor(i);

                if (valor is not null && valor != 0)
                {
                    valorDouble = Convert.ToDouble(valor);
                    valor = Convert.ToSingle(Math.Log10(valorDouble));
                }

                resultado.Add(valor);
            }

            dataFrame.AlterarColuna(Operacao.col, resultado);

            return dataFrame;
        }
    }
}
