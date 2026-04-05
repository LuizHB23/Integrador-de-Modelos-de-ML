using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.ExpressionsNo;
using IntegradorDominio.DataFrameModel;
using System.Linq.Expressions;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.Line
{
    public class ColunaAtribuicaoExpression : NodeExpression
    {
        public string NomeColuna { get; set; }
        public string NomeDataFrame { get; set; }
        public NodeExpression Valor { get; set; }
        public Type TipoDado { get; set; }

        public ColunaAtribuicaoExpression(string nomeDataFrame, string nomeColuna, NodeExpression valor, Type tipoDado)
        {
            NomeDataFrame = nomeDataFrame;
            NomeColuna = nomeColuna;
            Valor = valor;
            TipoDado = tipoDado;
        }

        public override Expression ParaExpression(Dictionary<string, ParameterExpression> variaveis,
            Dictionary<string, object> contexto, ParameterExpression indexVar)
        {
            if (!contexto.TryGetValue(NomeDataFrame, out var dfObj) || dfObj is not DataFrame df)
                throw new Exception($"DataFrame '{NomeDataFrame}' não encontrado no contexto");

            var dfParam = Expression.Constant(dfObj, dfObj.GetType());

            var pegarColuna = Expression.Call(
                dfParam,
                df.GetType().GetMethod("PegarColuna")!.MakeGenericMethod(TipoDado),
                Expression.Constant(NomeColuna)
            );

            var valorExpr = Valor.ParaExpression(variaveis, contexto, indexVar);

            // converte para object apenas para InjetarValor
            var valorConvertido = Expression.Convert(valorExpr, typeof(object));

            var metodoInjetar = typeof(Coluna<>).MakeGenericType(TipoDado).GetMethod("InjetarValor")!;
            return Expression.Call(pegarColuna, metodoInjetar, indexVar, valorConvertido);
        }
    }
}
