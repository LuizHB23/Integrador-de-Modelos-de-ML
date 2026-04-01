using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesEspeciais;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEspeciais
{
    public class FloorExecutor : FeatureExecutorBase<Floor>
    {
        public FloorExecutor(Floor operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColuna<Single?>(Operacao.col);
            int quantidadeLinhas = dataFrame.QuantidadeLinhas;
            var resultado = new List<Single?>();
            Single? valor;
            decimal valorDecimal;

            for (int i = 0; i < quantidadeLinhas; i++)
            {
                valor = (Single?)coluna.PegarValor(i);

                if (valor is not null)
                {
                    valorDecimal = Convert.ToDecimal(valor);
                    valor = Convert.ToSingle(Math.Floor(valorDecimal));
                }

                resultado.Add(valor);
            }

            dataFrame.AlterarColuna(Operacao.col, resultado);

            return dataFrame;
        }
    }
}
