using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.AgrupamentoDados;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.AgrupamentoDados
{
    public class SortExecutor : FeatureExecutorBase<Sort>
    {
        public SortExecutor(Sort operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var nomeColuna = Operacao.col;
            var colunaBase = dataFrame.Colunas[dataFrame.ColunaIndex[nomeColuna]];

            int n = colunaBase.Quantidade;

            // Cria array de índices
            int[] indices = new int[n];
            for (int i = 0; i < n; i++) indices[i] = i;

            // Ordena índices considerando nulls no final (como Pandas)
            bool crescente = Operacao.asc?.ToLower() != "false";
            // padrão = crescente (como Pandas)

            Array.Sort(indices, (i1, i2) =>
            {
                var v1 = colunaBase.PegarValor(i1);
                var v2 = colunaBase.PegarValor(i2);

                if (v1 == null && v2 == null) return 0;
                if (v1 == null) return 1; // nulls sempre no final
                if (v2 == null) return -1;

                int resultado = ((IComparable)v1).CompareTo(v2);

                return crescente ? resultado : -resultado;
            });

            for (int c = 0; c < dataFrame.Colunas.Count; c++)
            {
                var colBase = dataFrame.Colunas[c];
                var novaLista = new List<object?>(n);

                for (int i = 0; i < n; i++)
                    novaLista.Add(colBase.PegarValor(indices[i]));

                var tipo = colBase.TipoDado;

                if (tipo == typeof(Single?))
                    dataFrame.AlterarColuna<Single?>(colBase.Nome, novaLista.ConvertAll(x => (Single?)x));
                else if (tipo == typeof(String))
                    dataFrame.AlterarColuna<String?>(colBase.Nome, novaLista.ConvertAll(x => (String?)x));
                else if (tipo == typeof(Boolean?))
                    dataFrame.AlterarColuna<Boolean?>(colBase.Nome, novaLista.ConvertAll(x => (Boolean?)x));
                else if (tipo == typeof(DateTime?))
                    dataFrame.AlterarColuna<DateTime?>(colBase.Nome, novaLista.ConvertAll(x => (DateTime?)x));
                else
                    throw new Exception($"Tipo de coluna {tipo.Name} não suportado para ordenação.");
            }

            return dataFrame;
        }
    }
}
