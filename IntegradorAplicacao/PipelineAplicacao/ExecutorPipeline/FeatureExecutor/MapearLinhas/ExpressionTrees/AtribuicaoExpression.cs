using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.ExpressionsNo;
using System.Linq.Expressions;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees
{
    public class AtribuicaoExpression : NodeExpression
    {
        public string Target { get; set; }
        public NodeExpression Valor { get; set; }

        public AtribuicaoExpression(string target, NodeExpression valor)
        {
            Target = target;
            Valor = valor;
        }

        public override Expression ParaExpression(Dictionary<string, ParameterExpression> variaveis, Dictionary<string, object> contexto, ParameterExpression indexVar)
        {
            if (!variaveis.ContainsKey(Target))
                variaveis[Target] = Expression.Parameter(typeof(int), Target);

            // Aqui chamamos ParaExpression do NodeExpression correto
            return Expression.Assign(variaveis[Target], Valor.ParaExpression(variaveis, contexto, indexVar));
        }
    }
}