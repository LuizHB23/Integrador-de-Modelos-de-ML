using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.AgrupamentoDados;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.AgrupamentoDados
{
    public class SortExecutor : FeatureExecutorBase<Sort>
    {
        public SortExecutor(Sort operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var colunasChave = TransformaStringColunasEmListaColunas(Operacao.col);
            bool crescente = Operacao.asc?.ToLower() != "false";

            int n = dataFrame.QuantidadeLinhas;

            int[] indices = new int[n];
            for (int i = 0; i < n; i++)
                indices[i] = i;

            var colunas = colunasChave
                .Select(c => dataFrame.PegarColunaBase(c))
                .ToArray();

            Array.Sort(indices, (i1, i2) =>
            {
                foreach (var col in colunas)
                {
                    var v1 = col.PegarValor(i1);
                    var v2 = col.PegarValor(i2);

                    if (v1 == null && v2 == null) continue;
                    if (v1 == null) return 1;
                    if (v2 == null) return -1;

                    int cmp = ((IComparable)v1).CompareTo(v2);
                    if (cmp != 0)
                        return crescente ? cmp : -cmp;
                }

                return 0;
            });

            // 🔥 REORDENA AS COLUNAS EXISTENTES (sem novo DataFrame)
            foreach (var col in dataFrame.Colunas)
            {
                col.Reordenar(indices); // <-- método ideal
            }

            return dataFrame;
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
