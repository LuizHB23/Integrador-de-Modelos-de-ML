using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.JanelasTemporais;
using IntegradorDominio.Models.DataFrameModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.JanelasTemporais
{
    public class GroupWindowExecutor : FeatureExecutorBase<GroupWindow>
    {
        public GroupWindowExecutor(GroupWindow operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var colunasChave = TransformaStringColunasEmListaColunas(Operacao.col);

            if (colunasChave.Count == 0)
                throw new Exception("É necessário informar pelo menos uma coluna-chave para o groupwindow.");

            int n = dataFrame.QuantidadeLinhas;

            // 🔥 HASH GROUP (mais rápido que string join + dictionary indireto)
            var grupos = new Dictionary<string, List<int>>(n / 4);

            for (int i = 0; i < n; i++)
            {
                var key = BuildKey(dataFrame, colunasChave, i);

                if (!grupos.TryGetValue(key, out var list))
                {
                    list = new List<int>(4);
                    grupos[key] = list;
                }

                list.Add(i);
            }

            var colunasNaoChave = dataFrame.ColunaIndex.Keys
                .Where(c => !colunasChave.Contains(c))
                .ToList();

            var resultados = new Dictionary<string, object?[]>(colunasNaoChave.Count);

            foreach (var col in colunasNaoChave)
                resultados[col] = new object?[n];

            // 🔥 PROCESSAMENTO POR GRUPO (SEM SORT GLOBAL)
            foreach (var grupo in grupos.Values)
            {
                for (int j = 0; j < grupo.Count; j++)
                {
                    int idx = grupo[j];

                    if (j == 0)
                    {
                        foreach (var col in colunasNaoChave)
                            resultados[col][idx] = null;

                        continue;
                    }

                    int prevIdx = grupo[j - 1];

                    foreach (var col in colunasNaoChave)
                    {
                        var colunaBase = dataFrame.PegarColunaBase(col);

                        var atual = colunaBase.PegarValor(idx);
                        var anterior = colunaBase.PegarValor(prevIdx);

                        if (atual is DateTime dtA && anterior is DateTime dtB)
                            resultados[col][idx] = (float)(dtA - dtB).TotalDays;

                        else if (atual is float fA && anterior is float fB)
                            resultados[col][idx] = fA - fB;

                        else
                            resultados[col][idx] = null;
                    }
                }
            }

            // 🔥 materialização final (igual pandas: novo dataframe)
            var tipoFinal = typeof(float?);

            foreach (var kvp in resultados)
            {
                var lista = (System.Collections.IList)
                    Activator.CreateInstance(typeof(List<>).MakeGenericType(tipoFinal))!;

                foreach (var v in kvp.Value)
                    lista.Add(v == null ? null : Convert.ToSingle(v));

                var metodo = typeof(DataFrame)
                    .GetMethod("AdicionarColuna")!
                    .MakeGenericMethod(tipoFinal);

                metodo.Invoke(dataFrame, new object[] { kvp.Key + "_diff", lista });
            }

            return dataFrame;
        }

        private string BuildKey(DataFrame df, List<string> cols, int row)
        {
            var sb = new StringBuilder(64);

            foreach (var c in cols)
            {
                var v = df.PegarColunaBase(c)?.PegarValor(row);
                sb.Append(v?.ToString() ?? "NULL");
                sb.Append('|');
            }

            return sb.ToString();
        }

        private List<string> TransformaStringColunasEmListaColunas(string colunas)
        {
            return colunas.Trim('[', ']')
                .Split(',')
                .Select(c => c.Trim().Trim('"'))
                .ToList();
        }
    }
}
