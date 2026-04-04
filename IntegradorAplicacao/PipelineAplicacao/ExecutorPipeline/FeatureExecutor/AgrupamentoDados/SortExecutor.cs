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
            if (colunasChave.Count == 0)
                throw new Exception("É necessário informar pelo menos uma coluna para ordenação.");

            int n = dataFrame.QuantidadeLinhas;

            // 🔹 2. Cria array de índices
            int[] indices = Enumerable.Range(0, n).ToArray();

            // 🔹 3. Determina ordem crescente ou decrescente
            bool crescente = Operacao.asc?.ToLower() != "false"; // padrão = crescente

            // 🔹 4. Ordena índices considerando múltiplas colunas
            Array.Sort(indices, (i1, i2) =>
            {
                foreach (var nomeCol in colunasChave)
                {
                    var coluna = dataFrame.PegarColunaBase(nomeCol);
                    var v1 = coluna.PegarValor(i1);
                    var v2 = coluna.PegarValor(i2);

                    if (v1 == null && v2 == null) continue;
                    if (v1 == null) return 1; // nulls no final
                    if (v2 == null) return -1;

                    int cmp = ((IComparable)v1).CompareTo(v2);
                    if (cmp != 0) return crescente ? cmp : -cmp;
                }
                return 0;
            });

            // 🔹 5. Reordenar todas as colunas dinamicamente
            var novoDataFrame = new DataFrame();
            foreach (var colBase in dataFrame.Colunas)
            {
                var tipo = colBase.TipoDado;
                var listaNova = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(tipo))!;

                for (int i = 0; i < n; i++)
                    listaNova.Add(colBase.PegarValor(indices[i]));

                var metodoAdicionar = typeof(DataFrame)
                    .GetMethod("AdicionarColuna")!
                    .MakeGenericMethod(tipo);

                metodoAdicionar.Invoke(novoDataFrame, new object[] { colBase.Nome, listaNova });
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
