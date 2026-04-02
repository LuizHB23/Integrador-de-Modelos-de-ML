using IntegradorDominio.FeatureEngineering.MapearLinhas.ExpressionsModelos;
using IntegradorDominio.FeatureEngineering.MapearLinhas.Variavel;
using System.Drawing;
using System.Linq.Expressions;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.Variaveis
{
    public class VariavelExpression : NodeExpression
    {
        public string Nome { get; set; }
        public NodeExpression Valor { get; set; }
        public int QuantidadeLinhas {  get; set; }

        public VariavelExpression(string nome, NodeExpression valor, int quantidadeLinhas)
        {
            Nome = nome;
            Valor = valor;
            QuantidadeLinhas = quantidadeLinhas;
        }

        public override Expression ParaExpression(Dictionary<string, ParameterExpression> variaveis, Dictionary<string, object> contexto, ParameterExpression indexVar)
        {
            if (!contexto.TryGetValue(Nome, out var varObj))
            {
                var novaVariavel = new Variavel<object>(Nome, QuantidadeLinhas);
                contexto[Nome] = novaVariavel;
                varObj = novaVariavel;
            }

            var varConst = Expression.Constant(varObj);

            var valorExpr = Valor.ParaExpression(variaveis, contexto, indexVar);

            var metodo = varObj.GetType().GetMethod("InjetarValor");

            return Expression.Call(
                varConst,
                metodo!,
                indexVar,
                Expression.Convert(valorExpr, typeof(object))
            );
        }
    }
}
