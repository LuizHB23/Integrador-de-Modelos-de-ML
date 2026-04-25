using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.ExpressionsNo;
using IntegradorDominio.DataFrameModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.Line
{
    public class ColunaExpression : NodeExpression
    {
        public string NomeColuna { get; set; }
        public string NomeDataFrame { get; set; }
        public Type TipoDado { get; set; }

        public ColunaExpression(string nomeDataFrame, string nomeColuna, Type tipoDado)
        {
            NomeDataFrame = nomeDataFrame;
            NomeColuna = nomeColuna;
            TipoDado = tipoDado;
        }

        public override Expression ParaExpression(Dictionary<string, ParameterExpression> variaveis, Dictionary<string, object> contexto, ParameterExpression indexVar)
        {
            if (!contexto.TryGetValue(NomeDataFrame, out var dfObj) || dfObj is not DataFrame df)
                throw new Exception($"DataFrame '{NomeDataFrame}' não encontrado no contexto");

            var dfParam = Expression.Constant(dfObj);

            // cache idealmente (static readonly)
            var metodoPegarColuna = typeof(DataFrame)
                .GetMethod("PegarColuna")!
                .MakeGenericMethod(TipoDado);

            var colunaExpr = Expression.Call(
                dfParam,
                metodoPegarColuna,
                Expression.Constant(NomeColuna)
            );

            // chama Get(index)
            var metodoGet = typeof(Coluna<>)
                .MakeGenericType(TipoDado)
                .GetMethod("Get")!;

            var valorExpr = Expression.Call(colunaExpr, metodoGet, indexVar);

            if (TipoDado.IsValueType && Nullable.GetUnderlyingType(TipoDado) == null)
                return Expression.Convert(valorExpr, typeof(Nullable<>).MakeGenericType(TipoDado));

            return valorExpr;
        }
    }
}
