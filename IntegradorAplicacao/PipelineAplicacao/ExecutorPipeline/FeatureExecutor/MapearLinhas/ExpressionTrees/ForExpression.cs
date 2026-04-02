using IntegradorDominio.FeatureEngineering.MapearLinhas.ExpressionsModelos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees
{
    public class ForExpression : NodeExpression
    {
        public NodeExpression Initializer { get; set; } // Ex: i = 0
        public NodeExpression Condition { get; set; }   // Ex: i < 10
        public NodeExpression Increment { get; set; }   // Ex: i = i + 1
        public List<NodeExpression> Body { get; set; } = new List<NodeExpression>();

        public override Expression ParaExpression(Dictionary<string, ParameterExpression> variaveis, Dictionary<string, object> contexto, ParameterExpression indexVar)
        {
            // Executa inicialização
            Expression initExpr = Initializer?.ParaExpression(variaveis, contexto, indexVar) ?? Expression.Empty();

            LabelTarget breakLabel = Expression.Label();

            // Corpo + incremento
            BlockExpression loopBody = Expression.Block(
                Body.ConvertAll(b => b.ParaExpression(variaveis, contexto, indexVar))
            );

            if (Increment != null)
                loopBody = Expression.Block(loopBody, Increment.ParaExpression(variaveis, contexto, indexVar));

            // Loop While simulando FOR
            Expression loop = Expression.Loop(
                Expression.IfThenElse(
                    Condition.ParaExpression(variaveis, contexto, indexVar),
                    loopBody,
                    Expression.Break(breakLabel)
                ),
                breakLabel
            );

            // Bloco final com inicialização + loop
            return Expression.Block(initExpr, loop);
        }
    }
}
