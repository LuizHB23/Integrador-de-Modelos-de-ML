using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesEstatisticas;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEstatisticas
{
    public class VarianciaExecutor : FeatureExecutorBase<Variancia>
    {
        public VarianciaExecutor(Variancia operacao) : base(operacao) { }

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

            Single media = 0;

            if (valoresValidos.Count > 0)
            {
                float soma = 0;

                foreach (var valor in valoresValidos)
                {
                    soma += valor;
                }

                media = soma / valoresValidos.Count;
            }

            Single variancia = 0;

            if (valoresValidos.Count > 1)
            {
                float somaQuadrada = 0;

                foreach (var valor in valoresValidos)
                {
                   somaQuadrada += (valor - media) * (valor - media);
                }

                variancia = somaQuadrada / (valoresValidos.Count);
            }

            return variancia;
        }
    }
}
