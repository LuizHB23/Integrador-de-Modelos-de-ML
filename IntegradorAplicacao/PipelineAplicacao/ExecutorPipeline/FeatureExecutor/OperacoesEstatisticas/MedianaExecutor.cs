using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesEstatisticas;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEstatisticas
{
    internal class MedianaExecutor : FeatureExecutorBase<Mediana>
    {
        public MedianaExecutor(Mediana operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColuna<Single?>(Operacao.col);
            int quantidadeLinhas = dataFrame.QuantidadeLinhas;
            var resultado = new List<Single?>();
            var valoresValidos = new List<float>();

            for (int i = 0; i < quantidadeLinhas; i++)
            {
                var valor = coluna.PegarValor(i);

                if (valor is not null)
                {
                    valoresValidos.Add((Single)valor);
                }
            }

            Single mediana = 0;
            if (valoresValidos.Count > 0)
            {
                valoresValidos.Sort();
                int meio = valoresValidos.Count / 2;

                if (valoresValidos.Count % 2 == 0)
                {
                    mediana = (valoresValidos[meio - 1] + valoresValidos[meio]) / 2;
                }
                else
                {
                    mediana = valoresValidos[meio];
                }
            }

            return mediana;
        }
    }
}
