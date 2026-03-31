using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.LimpezaDados;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.LimpezaDados
{
    public class RemoverColunaExecutor : FeatureExecutorBase<RemoverColuna>
    {
        public RemoverColunaExecutor(RemoverColuna operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var colunasParaRemover = TransformaStringColunasEmListaColunas(Operacao.col);
            var novoDataFrame = new DataFrame();

            foreach (var coluna in dataFrame.Colunas)
            {
                if (!colunasParaRemover.Contains(coluna.Nome))
                {
                    novoDataFrame.Colunas.Add(coluna);
                }
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