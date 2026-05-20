using IntegradorDominio.FeatureEngineering.LimpezaDados;
using System.Linq.Expressions;
using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.LimpezaDados
{
    public class FiltrarExecutor : FeatureExecutorBase<Filtrar>
    {
        public FiltrarExecutor(Filtrar operacao) : base(operacao) { }

        public override object Executar(DataFrame df)
        {
            if (string.IsNullOrWhiteSpace(Operacao.condition))
                throw new Exception("É necessário informar uma condição.");

            var predicate = CriarPredicate(Operacao.condition, df);

            int n = df.QuantidadeLinhas;
            var mask = new bool[n];

            for (int i = 0; i < n; i++)
                mask[i] = predicate(i);

            var novo = new DataFrame();

            foreach (var col in df.Colunas)
            {
                var tipo = col.TipoDado;

                var lista = (System.Collections.IList)
                    Activator.CreateInstance(typeof(List<>).MakeGenericType(tipo))!;

                for (int i = 0; i < n; i++)
                    if (mask[i])
                        lista.Add(col.PegarValor(i));

                var metodo = typeof(DataFrame)
                    .GetMethod("AdicionarColuna")!
                    .MakeGenericMethod(tipo);

                metodo.Invoke(novo, new object[] { col.Nome, lista });
            }

            return novo;
        }

        // =========================
        // PREDICATE
        // =========================
        private Func<int, bool> CriarPredicate(string condition, DataFrame df)
        {
            var iParam = Expression.Parameter(typeof(int), "i");

            var expr = ParseExpression(condition, iParam, df);

            return Expression.Lambda<Func<int, bool>>(expr, iParam).Compile();
        }

        // =========================
        // PARSER
        // =========================
        private Expression ParseExpression(string condition, ParameterExpression iParam, DataFrame df)
        {
            condition = condition.Trim();

            // remove parênteses externos
            while (condition.StartsWith("(") && condition.EndsWith(")") &&
                   ParentesesBalanceados(condition[1..^1]))
            {
                condition = condition[1..^1].Trim();
            }

            // OR
            int idx = EncontrarOperadorExterno(condition, "||");
            if (idx >= 0)
            {
                var left = condition[..idx];
                var right = condition[(idx + 2)..];

                return Expression.OrElse(
                    ParseExpression(left, iParam, df),
                    ParseExpression(right, iParam, df)
                );
            }

            // AND
            idx = EncontrarOperadorExterno(condition, "&&");
            if (idx >= 0)
            {
                var left = condition[..idx];
                var right = condition[(idx + 2)..];

                return Expression.AndAlso(
                    ParseExpression(left, iParam, df),
                    ParseExpression(right, iParam, df)
                );
            }

            // IN / NOT IN
            if (condition.Contains("[") && condition.Contains("]"))
                return ParseInExpression(condition, iParam, df);

            // comparação simples
            return ParseComparison(condition, iParam, df);
        }

        // =========================
        // IN / NOT IN
        // =========================
        private Expression ParseInExpression(string condition, ParameterExpression iParam, DataFrame df)
        {
            bool notIn = condition.Contains("!=");
            var op = notIn ? "!=" : "==";

            var parts = condition.Split(op);

            if (parts.Length != 2)
                throw new Exception($"Expressão IN inválida: {condition}");

            var left = parts[0].Trim();
            var right = parts[1].Trim();

            // remove [ ]
            right = right.Trim().TrimStart('[').TrimEnd(']');

            var valores = right
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim().Trim('\'', '"'))
                .ToArray();

            var colunaExpr = CriarOperando(left, iParam, df);

            // HashSet (O(1))
            var hashSet = new HashSet<string>(valores, StringComparer.Ordinal);

            var containsMethod = typeof(HashSet<string>).GetMethod("Contains")!;

            var containsCall = Expression.Call(
                Expression.Constant(hashSet),
                containsMethod,
                Expression.Convert(colunaExpr, typeof(string))
            );

            return notIn
                ? Expression.Not(containsCall)
                : containsCall;
        }

        // =========================
        // COMPARAÇÃO
        // =========================
        private Expression ParseComparison(string condition, ParameterExpression iParam, DataFrame df)
        {
            string[] ops = new[] { ">=", "<=", "==", "!=", ">", "<" };

            foreach (var op in ops)
            {
                int idx = condition.IndexOf(op);
                if (idx < 0) continue;

                var left = condition[..idx].Trim();
                var right = condition[(idx + op.Length)..].Trim();

                var leftExpr = CriarOperando(left, iParam, df);
                var rightExpr = CriarOperando(right, iParam, df);

                return BuildComparison(leftExpr, rightExpr, op);
            }

            throw new Exception($"Condição inválida: {condition}");
        }

        // =========================
        // OPERANDO
        // =========================
        private Expression CriarOperando(string token, ParameterExpression iParam, DataFrame df)
        {
            token = token.Trim();

            // STRING
            if (token.StartsWith("'") && token.EndsWith("'"))
            {
                var val = token[1..^1];
                return Expression.Constant(val, typeof(string));
            }

            // COLUNA
            if (df.ColunaIndex.ContainsKey(token))
            {
                var coluna = df.PegarColunaBase(token)!;

                var metodo = coluna.GetType().GetMethod("PegarValor")!;

                var call = Expression.Call(
                    Expression.Constant(coluna),
                    metodo,
                    iParam
                );

                return Expression.Convert(call, coluna.TipoDado);
            }

            // NUMERO
            if (float.TryParse(token, out var num))
                return Expression.Constant(num, typeof(float));

            // BOOL
            if (bool.TryParse(token, out var b))
                return Expression.Constant(b, typeof(bool));

            throw new Exception($"Token inválido: {token}");
        }

        // =========================
        // HELPERS
        // =========================
        private bool ParentesesBalanceados(string s)
        {
            int count = 0;

            foreach (var c in s)
            {
                if (c == '(') count++;
                else if (c == ')') count--;

                if (count < 0) return false;
            }

            return count == 0;
        }

        private int EncontrarOperadorExterno(string cond, string op)
        {
            int depth = 0;

            for (int i = 0; i <= cond.Length - op.Length; i++)
            {
                if (cond[i] == '(') depth++;
                else if (cond[i] == ')') depth--;

                if (depth == 0 && cond.Substring(i, op.Length) == op)
                    return i;
            }

            return -1;
        }

        private Expression BuildComparison(Expression left, Expression right, string op)
        {
            var leftUnderlying = Nullable.GetUnderlyingType(left.Type);
            var rightUnderlying = Nullable.GetUnderlyingType(right.Type);

            bool leftNullable = leftUnderlying != null;
            bool rightNullable = rightUnderlying != null;

            // =========================
            // CASO: ambos nullable
            // =========================
            if (leftNullable && rightNullable)
            {
                var lVal = Expression.Property(left, "Value");
                var rVal = Expression.Property(right, "Value");

                var lHas = Expression.Property(left, "HasValue");
                var rHas = Expression.Property(right, "HasValue");

                var comparison = BuildNonNullableComparison(lVal, rVal, op);

                return Expression.AndAlso(
                    Expression.AndAlso(lHas, rHas),
                    comparison
                );
            }

            // =========================
            // CASO: left nullable
            // =========================
            if (leftNullable)
            {
                var lVal = Expression.Property(left, "Value");
                var lHas = Expression.Property(left, "HasValue");

                var rightConverted = Expression.Convert(right, lVal.Type);

                var comparison = BuildNonNullableComparison(lVal, rightConverted, op);

                return Expression.AndAlso(lHas, comparison);
            }

            // =========================
            // CASO: right nullable
            // =========================
            if (rightNullable)
            {
                var rVal = Expression.Property(right, "Value");
                var rHas = Expression.Property(right, "HasValue");

                var leftConverted = Expression.Convert(left, rVal.Type);

                var comparison = BuildNonNullableComparison(leftConverted, rVal, op);

                return Expression.AndAlso(rHas, comparison);
            }

            // =========================
            // CASO: nenhum nullable
            // =========================
            return BuildNonNullableComparison(left, right, op);
        }

        private Expression BuildNonNullableComparison(Expression left, Expression right, string op)
        {
            return op switch
            {
                ">" => Expression.GreaterThan(left, right),
                "<" => Expression.LessThan(left, right),
                ">=" => Expression.GreaterThanOrEqual(left, right),
                "<=" => Expression.LessThanOrEqual(left, right),
                "==" => Expression.Equal(left, right),
                "!=" => Expression.NotEqual(left, right),
                _ => throw new Exception($"Operador inválido: {op}")
            };
        }
    }
}