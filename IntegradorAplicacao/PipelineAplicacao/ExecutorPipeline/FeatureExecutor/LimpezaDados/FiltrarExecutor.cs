using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.LimpezaDados;
using IntegradorDominio.DataFrameModel;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.LimpezaDados
{
    public class FiltrarExecutor : FeatureExecutorBase<Filtrar>
    {
        public FiltrarExecutor(Filtrar operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            if (string.IsNullOrWhiteSpace(Operacao.condition))
                throw new Exception("Condição inválida");

            var plan = Parse(Operacao.condition, dataFrame);

            var mask = new bool[dataFrame.QuantidadeLinhas];

            var col = plan.Column != null
                ? dataFrame.PegarColunaBase(plan.Column)
                : null;

            for (int i = 0; i < dataFrame.QuantidadeLinhas; i++)
            {
                mask[i] = Evaluate(plan, col, i);
            }

            // materializa novo DataFrame
            var novo = new DataFrame();

            foreach (var c in dataFrame.Colunas)
            {
                var tipo = c.TipoDado;
                var listType = typeof(List<>).MakeGenericType(tipo);
                var list = (System.Collections.IList)Activator.CreateInstance(listType)!;

                for (int i = 0; i < mask.Length; i++)
                {
                    if (!mask[i]) continue;
                    list.Add(c.PegarValor(i));
                }

                var method = typeof(DataFrame)
                    .GetMethod("AdicionarColuna")!
                    .MakeGenericMethod(tipo);

                method.Invoke(novo, new object[] { c.Nome, list });
            }

            return novo;
        }

        // ============================
        // PARSER SIMPLES (IN / NOT IN)
        // ============================
        private FilterPlan Parse(string condition, DataFrame df)
        {
            condition = condition.Trim();

            bool isNotIn = condition.Contains("!=");

            var parts = condition.Split(isNotIn ? "!=" : "==");

            var column = parts[0].Trim();

            var rawList = parts[1]
                .Trim()
                .Trim('[', ']');

            // split seguro
            var values = rawList
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim().Trim('\'', '"'))
                .ToHashSet(StringComparer.Ordinal);

            return new FilterPlan
            {
                Column = column,
                Set = values,
                IsNotIn = isNotIn
            };
        }

        // ============================
        // EXECUTOR O(1) POR LINHA
        // ============================
        private bool Evaluate(FilterPlan plan, ColunaBase col, int row)
        {
            var value = col.PegarValor(row)?.ToString();

            if (value == null)
                return false;

            bool contains = plan.Set.Contains(value);

            return plan.IsNotIn ? !contains : contains;
        }

        // ============================
        // STRUCT (zero GC extra)
        // ============================
        private struct FilterPlan
        {
            public string Column;
            public HashSet<string> Set;
            public bool IsNotIn;
        }
    }
}