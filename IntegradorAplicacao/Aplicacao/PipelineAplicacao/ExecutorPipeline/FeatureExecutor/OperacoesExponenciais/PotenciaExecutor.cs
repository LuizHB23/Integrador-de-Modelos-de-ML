using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.OperacoesExponenciais;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesExponenciais
{
    public class PotenciaExecutor : FeatureExecutorBase<Potencia>
    {
        public PotenciaExecutor(Potencia operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColuna<Single?>(Operacao.col);

            var span = coluna.PegarColunaSpan();
            int n = span.Length;

            int potencia = Convert.ToInt32(Operacao.value);

            for (int i = 0; i < n; i++)
            {
                var v = span[i];

                if (v.HasValue)
                    span[i] = (float)Math.Pow(v.Value, potencia);
                else
                    span[i] = null;
            }

            return dataFrame;
        }
    }
}
