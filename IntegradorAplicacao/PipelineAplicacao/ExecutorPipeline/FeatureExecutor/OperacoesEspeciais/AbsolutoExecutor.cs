using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesEspeciais;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEspeciais
{
    public class AbsolutoExecutor : FeatureExecutorBase<Absoluto>
    {
        public AbsolutoExecutor(Absoluto operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var colunaBase = dataFrame.PegarColunaBase(Operacao.col);

            if (colunaBase is not Coluna<Single?> col)
                throw new Exception("Coluna inválida para operação absoluto.");

            var span = col.PegarColunaSpan();

            for (int i = 0; i < span.Length; i++)
            {
                var valor = span[i];

                if (valor.HasValue && valor.Value < 0)
                    span[i] = -valor;
            }

            return dataFrame;
        }
    }
}
