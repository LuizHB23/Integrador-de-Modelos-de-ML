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

            var resultado = new float[n];

            for (int i = 0; i < n; i++)
            {
                //resultado[i] =  Operacao.ColunaEsquerda.Dados[i] + Operacao.ColunaDireita.Dados[i];
            }

            //dataFrame.AddColumn(Operacao.NomeColunaSaida, resultado);

            return dataFrame;
        }
    }
}
