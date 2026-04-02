using IntegradorDominio.FeatureEngineering.MapearLinhas.ExpressionsModelos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees
{
    public class ValueExpression : NodeExpression
    {
        public object? Valor { get; set; }
        public ValueExpression(object valor)
        {
            Valor = valor;
        }

        public override Expression ParaExpression(Dictionary<string, ParameterExpression> variaveis, Dictionary<string, object> contexto, ParameterExpression indexVar)
        {
            return Expression.Constant(Valor);
        }
    }
}
