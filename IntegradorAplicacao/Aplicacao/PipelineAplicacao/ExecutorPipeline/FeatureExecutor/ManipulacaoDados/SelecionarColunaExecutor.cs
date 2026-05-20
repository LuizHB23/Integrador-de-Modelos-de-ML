using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.ManipulacaoDados
{
    public class SelecionarColunaExecutor : FeatureExecutorBase<SelecionarColuna>
    {
        public SelecionarColunaExecutor(SelecionarColuna operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var novoDataFrame = new DataFrame();
            var colunas = TransformaStringColunasEmListaColunas(Operacao.col);

            var colunasDesejadas = new HashSet<string>(colunas);

            foreach (var coluna in dataFrame.Colunas)
            {
                if (!colunasDesejadas.Contains(coluna.Nome))
                    continue;

                var novaColuna = coluna.Clonar();

                novoDataFrame.Colunas.Add(novaColuna);
                novoDataFrame.ColunaIndex[novaColuna.Nome] = novoDataFrame.Colunas.Count - 1;
            }

            return novoDataFrame;
        }

        private List<string> TransformaStringColunasEmListaColunas(string colunas)
        {
            var texto = colunas.Trim('[', ']').Split(',');
            List<string> colunasParaRemover = new();

            foreach (var coluna in texto)
            {
                colunasParaRemover.Add(coluna.Trim().Trim('"'));
            }

            return colunasParaRemover;
        }
    }
}
