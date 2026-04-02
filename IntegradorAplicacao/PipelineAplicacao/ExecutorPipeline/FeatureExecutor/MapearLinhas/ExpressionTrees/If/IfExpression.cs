using IntegradorDominio.FeatureEngineering.MapearLinhas.ExpressionsModelos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.If
{
    public class IfExpression : NodeExpression
    {
        public NodeExpression Condition { get; set; }
        public List<NodeExpression> Body { get; set; } = new List<NodeExpression>(); // Corpo do IF
        public List<NodeExpression> ElseBody { get; set; } = new List<NodeExpression>(); // Corpo do ELSE (opcional)

        public IfExpression(NodeExpression condition)
        {
            Condition = condition;
        }

        public override Expression ParaExpression(Dictionary<string, ParameterExpression> variaveis, Dictionary<string, object> contexto, ParameterExpression indexVar)
        {
            // Corpo do IF
            BlockExpression bodyBlock = Expression.Block(
                Body.ConvertAll(b => b.ParaExpression(variaveis, contexto, indexVar))
            );

            // Corpo do ELSE (opcional)
            Expression elseExpr = ElseBody.Count > 0
                ? Expression.Block(ElseBody.ConvertAll(b => b.ParaExpression(variaveis, contexto, indexVar)))
                : Expression.Empty();

            // IF com ELSE
            if (ElseBody.Count > 0)
                return Expression.IfThenElse(
                    Condition.ParaExpression(variaveis, contexto, indexVar),
                    bodyBlock,
                    elseExpr
                );
            else
                return Expression.IfThen(
                    Condition.ParaExpression(variaveis, contexto, indexVar),
                    bodyBlock
                );
        }
    }
}
