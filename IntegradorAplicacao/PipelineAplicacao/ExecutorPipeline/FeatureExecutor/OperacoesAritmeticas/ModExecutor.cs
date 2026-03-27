using IntegradorAplicacao.PipelineAplicacao.Interfaces;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesAritmeticas
{
    public class ModExecutor : IFeatureExecutor<Mod>
    {
        public Mod Operacao { get; }

        public ModExecutor(Mod operacao)
        {
            Operacao = operacao;
        }

        public DataFrame Executar(DataFrame dataFrame)
        {
            var n = dataFrame.QuantidadeLinhas;

            var resultado = new float[n];

            for (int i = 0; i < n; i++)
            {
                resultado[i] = Operacao.Coluna.Dados[i] % Operacao.Divisor;
            }

            dataFrame.AddColumn(Operacao.NomeColunaSaida, resultado);

            return dataFrame;
        }
    }
}
