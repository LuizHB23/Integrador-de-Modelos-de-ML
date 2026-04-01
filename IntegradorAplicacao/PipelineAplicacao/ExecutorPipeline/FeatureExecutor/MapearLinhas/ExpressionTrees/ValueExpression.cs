using IntegradorDominio.FeatureEngineering.MapearLinhas.ExpressionsModelos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees
{
    public class ValueExpression : NodeExpression
    {
        public string Nome { get; set; } // variável ou valor
        public ValueExpression(string nome) => Nome = nome;

        public override Expression ParaExpression(Dictionary<string, ParameterExpression> variaveis)
        {
            if (int.TryParse(Nome, out int literal))
            {
                return Expression.Constant(literal);
            }

            if (!variaveis.ContainsKey(Nome))
            {
                variaveis[Nome] = Expression.Parameter(typeof(int), Nome);
            }

            return variaveis[Nome];
        }
    }
}
