using IntegradorDominio.FeatureEngineering.MapearLinhas.ExpressionsModelos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees
{
    public class BinarioExpression : NodeExpression
    {
        public NodeExpression Left { get; set; }
        public NodeExpression Right { get; set; }
        public string Operador { get; set; } // "+", "-", "*", "/", ">", "<", "=="

        public BinarioExpression(NodeExpression left, string operador, NodeExpression right)
        {
            Left = left;
            Operador = operador;
            Right = right;
        }

        public override Expression ParaExpression(Dictionary<string, ParameterExpression> variaveis, Dictionary<string, object> contexto, ParameterExpression indexVar)
        {

            var leftExpr = Left.ParaExpression(variaveis, contexto, indexVar);
            var rightExpr = Right.ParaExpression(variaveis, contexto, indexVar);

            var tipoLeft = leftExpr.Type;
            var tipoRight = rightExpr.Type;

            // Se os tipos forem diferentes, converte ambos para float?
            if (tipoLeft != tipoRight)
            {
                leftExpr = Expression.Convert(leftExpr, typeof(float?));
                rightExpr = Expression.Convert(rightExpr, typeof(float?));
            }

            return Operador switch
            {
                "+" => Expression.Add(leftExpr, rightExpr),
                "-" => Expression.Subtract(leftExpr, rightExpr),
                "*" => Expression.Multiply(leftExpr, rightExpr),
                "/" => Expression.Divide(leftExpr, rightExpr),
                ">" => Expression.GreaterThan(leftExpr, rightExpr),
                "<" => Expression.LessThan(leftExpr, rightExpr),
                ">=" => Expression.GreaterThanOrEqual(leftExpr, rightExpr),
                "<=" => Expression.LessThanOrEqual(leftExpr, rightExpr),
                "==" => Expression.Equal(leftExpr, rightExpr),
                "!=" => Expression.NotEqual(leftExpr, rightExpr),
                _ => throw new Exception($"Operador desconhecido: {Operador}")
            };
        }
    }
}
