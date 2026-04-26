using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.AgrupamentoDados;
using System.Collections;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.AgrupamentoDados
{
    public class GroupByExecutor : FeatureExecutorBase<GroupBy>
    {
        public GroupByExecutor(GroupBy operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var colunasChave = TransformaStringColunasEmListaColunas(Operacao.col);
            var agg = Operacao.agg?.ToLower();

            if (colunasChave.Count == 0)
                throw new Exception("groupby requer colunas");

            if (agg == "diff")
                return ExecutarDiffComoJanela(dataFrame, colunasChave);

            var keys = BuildKeys(dataFrame, colunasChave);

            var indices = Enumerable.Range(0, dataFrame.QuantidadeLinhas)
                                    .OrderBy(i => keys[i])
                                    .ToArray();


            var result = new Dictionary<string, List<object?>>();
            foreach (var c in dataFrame.Colunas)
                result[c.Nome] = new List<object?>();

            Func<ColunaBase, int[], object?> aggFunc = agg switch
            {
                "sum" => AgregacaoSoma,
                "count" => AgregacaoCount,
                "mean" => AgregacaoMedia,
                "std" => AgregacaoDesvioPadrao,
                "min" => AgregacaoMinimo,
                "max" => AgregacaoMaximo,
                _ => throw new Exception("agg não suportado")
            };

            // 🔥 3. SCAN GROUPS (CORE OPTIMIZATION)
            int start = 0;

            while (start < indices.Length)
            {
                int end = start + 1;

                while (end < indices.Length &&
                       keys[indices[end]] == keys[indices[start]])
                    end++;

                var slice = new int[end - start];
                Array.Copy(indices, start, slice, 0, slice.Length);

                foreach (var col in dataFrame.Colunas)
                {
                    if (colunasChave.Contains(col.Nome))
                    {
                        result[col.Nome].Add(col.PegarValor(slice[0]));
                    }
                    else
                    {
                        result[col.Nome].Add(aggFunc(col, slice));
                    }
                }

                start = end;
            }

            // 🔥 4. BUILD DF
            var novo = new DataFrame();

            foreach (var col in dataFrame.Colunas)
            {
                AdicionarColunaTipadaDynamic(
                    novo,
                    col.Nome,
                    result[col.Nome],
                    col.TipoDado,
                    false
                );
            }

            return novo;
        }

        private string[] BuildKeys(DataFrame df, List<string> cols)
        {
            var keys = new string[df.QuantidadeLinhas];
            var sb = new StringBuilder(64);

            for (int i = 0; i < df.QuantidadeLinhas; i++)
            {
                sb.Clear();

                foreach (var c in cols)
                {
                    var v = df.PegarColunaBase(c)?.PegarValor(i);
                    sb.Append(v ?? "NULL").Append('|');
                }

                keys[i] = sb.ToString();
            }

            return keys;
        }

        private object? AgregacaoSoma(ColunaBase col, int[] idx)
        {
            if (col is Coluna<float?> c)
            {
                var span = c.PegarColunaSpan();
                float sum = 0;

                for (int i = 0; i < idx.Length; i++)
                {
                    var v = span[idx[i]];
                    if (v.HasValue) sum += v.Value;
                }

                return sum;
            }

            float s = 0;
            foreach (var i in idx)
            {
                var v = (float?)col.PegarValor(i);
                if (v.HasValue) s += v.Value;
            }

            return s;
        }

        private object? AgregacaoCount(ColunaBase col, int[] idx)
        {
            int c = 0;

            foreach (var i in idx)
                if (col.PegarValor(i) != null)
                    c++;

            return c;
        }

        private object? AgregacaoMedia(ColunaBase col, int[] idx)
        {
            if (col is Coluna<float?> c)
            {
                var span = c.PegarColunaSpan();

                float sum = 0;
                int count = 0;

                for (int i = 0; i < idx.Length; i++)
                {
                    var v = span[idx[i]];
                    if (v.HasValue)
                    {
                        sum += v.Value;
                        count++;
                    }
                }

                return count == 0 ? null : sum / count;
            }

            float s = 0;
            int n = 0;

            foreach (var i in idx)
            {
                var v = (float?)col.PegarValor(i);
                if (v.HasValue)
                {
                    s += v.Value;
                    n++;
                }
            }

            return n == 0 ? null : s / n;
        }

        private object? AgregacaoMinimo(ColunaBase col, int[] idx)
        {
            float? min = null;

            if (col is Coluna<float?> c)
            {
                var span = c.PegarColunaSpan();

                for (int i = 0; i < idx.Length; i++)
                {
                    var v = span[idx[i]];
                    if (!v.HasValue) continue;

                    if (min == null || v.Value < min)
                        min = v;
                }

                return min;
            }

            foreach (var i in idx)
            {
                var v = (float?)col.PegarValor(i);
                if (!v.HasValue) continue;

                if (min == null || v < min)
                    min = v;
            }

            return min;
        }

        private object? AgregacaoMaximo(ColunaBase col, int[] idx)
        {
            float? max = null;

            if (col is Coluna<float?> c)
            {
                var span = c.PegarColunaSpan();

                for (int i = 0; i < idx.Length; i++)
                {
                    var v = span[idx[i]];
                    if (!v.HasValue) continue;

                    if (max == null || v.Value > max)
                        max = v;
                }

                return max;
            }

            foreach (var i in idx)
            {
                var v = (float?)col.PegarValor(i);
                if (!v.HasValue) continue;

                if (max == null || v > max)
                    max = v;
            }

            return max;
        }

        private object? AgregacaoDesvioPadrao(ColunaBase col, int[] idx)
        {
            var vals = new List<double>(idx.Length);

            foreach (var i in idx)
            {
                var v = col.PegarValor(i);
                if (v != null)
                    vals.Add(Convert.ToDouble(v));
            }

            if (vals.Count <= 1) return null;

            double mean = vals.Sum() / vals.Count;
            double var = vals.Sum(x => (x - mean) * (x - mean)) / (vals.Count - 1);

            return (float)Math.Sqrt(var);
        }

        private object ExecutarDiffComoJanela(DataFrame df, List<string> cols)
        {
            var keys = BuildKeys(df, cols);

            var indices = Enumerable.Range(0, df.QuantidadeLinhas)
                .OrderBy(i => keys[i])
                .ToArray();

            var novo = new DataFrame();

            foreach (var col in df.Colunas)
            {
                var values = new object?[df.QuantidadeLinhas];

                if (cols.Contains(col.Nome))
                {
                    for (int i = 0; i < df.QuantidadeLinhas; i++)
                        values[i] = col.PegarValor(i);

                    AdicionarColunaTipadaDynamic(novo, col.Nome, values.ToList(), col.TipoDado, false);
                    continue;
                }

                if (col is Coluna<float?> c)
                {
                    var span = c.PegarColunaSpan();

                    string lastKey = null;
                    float? lastVal = null;

                    for (int i = 0; i < indices.Length; i++)
                    {
                        int idx = indices[i];
                        var key = keys[idx];

                        var v = span[idx];

                        if (key != lastKey)
                        {
                            values[idx] = null;
                            lastKey = key;
                            lastVal = v;
                            continue;
                        }

                        values[idx] = (v.HasValue && lastVal.HasValue)
                            ? v.Value - lastVal.Value
                            : null;

                        lastVal = v;
                    }
                }

                AdicionarColunaTipadaDynamic(novo, col.Nome, values.ToList(), col.TipoDado, true);
            }

            return novo;
        }

        private void AdicionarColunaTipadaDynamic(
            DataFrame df,
            string nome,
            List<object?> valores,
            Type tipoOriginal,
            bool ehDiff = false)
        {
            Type tipoFinal = ehDiff ? typeof(float?) : tipoOriginal;

            var lista = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(tipoFinal))!;

            foreach (var v in valores)
                lista.Add(v == null ? null : Convert.ChangeType(v, Nullable.GetUnderlyingType(tipoFinal) ?? tipoFinal));

            var metodo = typeof(DataFrame)
                .GetMethod("AdicionarColuna")!
                .MakeGenericMethod(tipoFinal);

            metodo.Invoke(df, new object[] { nome, lista });
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