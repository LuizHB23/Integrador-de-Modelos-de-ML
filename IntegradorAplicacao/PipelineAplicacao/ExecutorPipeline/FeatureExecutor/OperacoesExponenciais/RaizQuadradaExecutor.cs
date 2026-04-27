using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesExponenciais;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesExponenciais
{
    public class RaizQuadradaExecutor : FeatureExecutorBase<RaizQuadrada>
    {
        public RaizQuadradaExecutor(RaizQuadrada operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColuna<Single?>(Operacao.col);
            var span = coluna.Dados.AsSpan(0, coluna.Quantidade);

            for (int i = 0; i < span.Length; i++)
            {
                ref var valor = ref span[i];

                if (valor is not null && valor >= 0)
                {
                    valor = (Single)MathF.Sqrt(valor.Value);
                }
            }

            return dataFrame;
        }
    }
}
