using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.LimpezaDados;
using IntegradorDominio.DataFrameModel;
using System.Linq.Expressions;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.LimpezaDados
{
    public class FiltrarExecutor : FeatureExecutorBase<Filtrar>
    {
        public FiltrarExecutor(Filtrar operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            if (string.IsNullOrWhiteSpace(Operacao.condition))
                throw new Exception("É necessário informar uma condição para o filtro.");

            // Cria predicado dinâmico
            var predicate = CriarPredicate(Operacao.condition, dataFrame);

            // Filtra linhas
            var linhasFiltradas = new List<int>();
            for (int i = 0; i < dataFrame.QuantidadeLinhas; i++)
            {
                var row = dataFrame.Colunas.ToDictionary(c => c.Nome, c => c.PegarValor(i));
                if (predicate(row))
                    linhasFiltradas.Add(i);
            }

            // Cria novo DataFrame com linhas filtradas
            var novoDataFrame = new DataFrame();
            foreach (var col in dataFrame.Colunas)
            {
                Type tipo = col.TipoDado; // tipo original da coluna

                // Cria uma lista tipada dinamicamente
                var listaTipada = (System.Collections.IList)Activator.CreateInstance(
                    typeof(List<>).MakeGenericType(tipo)
                )!;

                foreach (var idx in linhasFiltradas)
                {
                    var valor = col.PegarValor(idx);

                    object valorConvertido;

                    if (valor == null)
                    {
                        valorConvertido = null;
                    }
                    else
                    {
                        // Converte para o tipo da coluna, incluindo Nullable<>
                        valorConvertido = Convert.ChangeType(valor, Nullable.GetUnderlyingType(tipo) ?? tipo);
                    }

                    listaTipada.Add(valorConvertido);
                }

                // Chama o método genérico AddColumn dinamicamente
                var metodoAdicionar = typeof(DataFrame)
                    .GetMethod("AdicionarColuna")!
                    .MakeGenericMethod(tipo);

                metodoAdicionar.Invoke(novoDataFrame, new object[] { col.Nome, listaTipada });
            }

            return novoDataFrame;
        }

        private Func<Dictionary<string, object?>, bool> CriarPredicate(string condition, DataFrame df)
        {
            var param = Expression.Parameter(typeof(Dictionary<string, object?>), "row");
            var expr = ParseExpression(condition, param, df);
            return Expression.Lambda<Func<Dictionary<string, object?>, bool>>(expr, param).Compile();
        }

        // Parse recursivo com suporte a parênteses e operadores lógicos
        private Expression ParseExpression(string condition, ParameterExpression param, DataFrame df)
        {
            condition = condition.Trim();

            // Remove parênteses externos
            while (condition.StartsWith("(") && condition.EndsWith(")") && ParentesesBalanceados(condition.Substring(1, condition.Length - 2)))
            {
                condition = condition.Substring(1, condition.Length - 2).Trim();
            }

            // Primeiro tenta || (menor precedência)
            int idx = EncontrarOperadorExterno(condition, "||");
            if (idx >= 0)
            {
                var left = condition.Substring(0, idx).Trim();
                var right = condition.Substring(idx + 2).Trim();
                return Expression.OrElse(ParseExpression(left, param, df), ParseExpression(right, param, df));
            }

            // Depois &&
            idx = EncontrarOperadorExterno(condition, "&&");
            if (idx >= 0)
            {
                var left = condition.Substring(0, idx).Trim();
                var right = condition.Substring(idx + 2).Trim();
                return Expression.AndAlso(ParseExpression(left, param, df), ParseExpression(right, param, df));
            }

            // Comparações
            string[] comparadores = new[] { ">=", "<=", "==", "!=", ">", "<" };
            foreach (var op in comparadores)
            {
                idx = condition.IndexOf(op);
                if (idx >= 0)
                {
                    var left = condition.Substring(0, idx).Trim();
                    var right = condition.Substring(idx + op.Length).Trim();
                    var leftExpr = CriarOperando(left, param, df);
                    var rightExpr = CriarOperando(right, param, df);

                    return op switch
                    {
                        ">" => Expression.GreaterThan(leftExpr, rightExpr),
                        "<" => Expression.LessThan(leftExpr, rightExpr),
                        ">=" => Expression.GreaterThanOrEqual(leftExpr, rightExpr),
                        "<=" => Expression.LessThanOrEqual(leftExpr, rightExpr),
                        "==" => Expression.Equal(leftExpr, rightExpr),
                        "!=" => Expression.NotEqual(leftExpr, rightExpr),
                        _ => throw new Exception($"Operador {op} não suportado")
                    };
                }
            }

            throw new Exception($"Não foi possível interpretar a condição: {condition}");
        }

        // Cria expressão para colunas ou valores literais
        private Expression CriarOperando(string token, ParameterExpression param, DataFrame df)
        {
            if (df.ColunaIndex.ContainsKey(token))
            {
                var coluna = df.PegarColunaBase(token);
                Type tipo = coluna.TipoDado; // Ex: Single?, DateTime?, bool?
                Type tipoBase = Nullable.GetUnderlyingType(tipo) ?? tipo;

                Expression acesso = Expression.Property(param, "Item", Expression.Constant(token));

                if (tipoBase == typeof(DateTime) || tipoBase == typeof(DateTime?))
                {
                    return Expression.Convert(acesso, typeof(DateTime?));
                }

                if (tipoBase == typeof(Single) || tipoBase == typeof(Single?))
                {
                    return Expression.Convert(acesso, typeof(Single?));
                }

                if (tipoBase == typeof(bool) || tipoBase == typeof(bool?))
                {
                    return Expression.Convert(acesso, typeof(bool?));
                }

                throw new Exception($"Tipo da coluna {token} não suportado para filtro");
            }

            // Literal numérico
            if (Single.TryParse(token, out Single numero))
                return Expression.Constant(numero, typeof(Single?));

            // Literal booleano
            if (bool.TryParse(token, out bool booleano))
                return Expression.Constant(booleano, typeof(bool?));

            throw new Exception($"Não foi possível interpretar token: {token}");
        }

        // Verifica se parênteses estão balanceados
        private bool ParentesesBalanceados(string s)
        {
            int count = 0;
            foreach (char c in s)
            {
                if (c == '(') count++;
                if (c == ')') count--;
                if (count < 0) return false;
            }
            return count == 0;
        }

        // Encontra operador lógico fora de parênteses
        private int EncontrarOperadorExterno(string condition, string operador)
        {
            int count = 0;
            for (int i = 0; i <= condition.Length - operador.Length; i++)
            {
                if (condition[i] == '(') count++;
                if (condition[i] == ')') count--;
                if (count == 0 && condition.Substring(i, operador.Length) == operador)
                    return i;
            }
            return -1;
        }
    }
}