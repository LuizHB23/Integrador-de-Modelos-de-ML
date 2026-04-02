using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.MapearLinhas.ExpressionsModelos;
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

            var dfParam = Expression.Constant(dfObj, dfObj.GetType());

            var pegarColuna = Expression.Call(
                dfParam,
                df.GetType().GetMethod("PegarColuna")!.MakeGenericMethod(TipoDado),
                Expression.Constant(NomeColuna)
            );

            var pegarValor = Expression.Call(
                pegarColuna,
                typeof(Coluna<>).MakeGenericType(TipoDado).GetMethod("Get")!,
                indexVar
            );

            // se for value type não-nullable, converte para Nullable
            if (TipoDado.IsValueType && Nullable.GetUnderlyingType(TipoDado) == null)
                return Expression.Convert(pegarValor, typeof(Nullable<>).MakeGenericType(TipoDado));

            return pegarValor;
        }
    }
}
