using IntegradorDominio.FeatureEngineering.MapearLinhas.ExpressionsModelos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees
{
    public class VariavelUsoExpression : NodeExpression
    {
        public string Nome { get; set; }

        public VariavelUsoExpression(string nome)
        {
            Nome = nome;
        }

        public override Expression ParaExpression(Dictionary<string, ParameterExpression> variaveis, Dictionary<string, object> contexto, ParameterExpression indexVar)
        {
            if (!contexto.TryGetValue(Nome, out var varObj))
            { 
                throw new Exception($"Variável '{Nome}' não existe");
            }

            var varConst = Expression.Constant(varObj);

            var metodo = varObj.GetType().GetMethod("PegarValor");

            return Expression.Call(
                varConst,
                metodo!,
                indexVar
            );
        }
    }
}
