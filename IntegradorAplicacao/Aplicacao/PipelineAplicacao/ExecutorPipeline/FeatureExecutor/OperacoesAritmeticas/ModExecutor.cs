using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesAritmeticas
{
    public class ModExecutor : FeatureExecutorBase<Mod>
    {
        public ModExecutor(Mod operacao) : base(operacao) { }

        public override DataFrame Executar(DataFrame dataFrame)
        {
            int n = dataFrame.QuantidadeLinhas;

            var coluna = dataFrame.PegarColuna<float?>(Operacao.col)
                ?? throw new Exception($"Coluna '{Operacao.col}' inválida.");

            var span = coluna.PegarColunaSpan();

            float divisor = Convert.ToSingle(Operacao.value);

            var resultado = new float?[n];

            for (int i = 0; i < n; i++)
            {
                var v = span[i];

                if (v.HasValue)
                    resultado[i] = v.Value % divisor;
            }

            dataFrame.AlterarColuna(Operacao.exit, resultado);

            return dataFrame;
        }
    }
}
