using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.ExpressionsNo;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.If
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

        public override Expression ParaExpression(
            Dictionary<string, ParameterExpression> variaveis,
            Dictionary<string, object> contexto,
            ParameterExpression indexVar)
        {
            // Constrói todas as expressões do corpo do IF
            var bodyExpressions = Body.ConvertAll(b => b.ParaExpression(variaveis, contexto, indexVar));
            var bodyBlock = Expression.Block(bodyExpressions);

            // Constrói todas as expressões do corpo do ELSE, se houver
            Expression elseBlock = ElseBody.Count > 0
                ? Expression.Block(ElseBody.ConvertAll(b => b.ParaExpression(variaveis, contexto, indexVar)))
                : Expression.Empty();

            // Retorna a expressão completa do IF
            if (ElseBody.Count > 0)
                return Expression.IfThenElse(
                    Condition.ParaExpression(variaveis, contexto, indexVar),
                    bodyBlock,
                    elseBlock
                );
            else
                return Expression.IfThen(
                    Condition.ParaExpression(variaveis, contexto, indexVar),
                    bodyBlock
                );
        }
    }
}
