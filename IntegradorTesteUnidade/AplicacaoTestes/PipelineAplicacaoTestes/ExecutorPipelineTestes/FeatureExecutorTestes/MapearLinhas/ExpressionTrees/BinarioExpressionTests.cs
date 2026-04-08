using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees;
using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.ExpressionsNo;
using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.Line;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.MapearLinhas.ExpressionTrees
{
    public class BinarioExpressionTests
    {
        private object Executar(NodeExpression node)
        {
            var variaveis = new Dictionary<string, ParameterExpression>();
            var contexto = new Dictionary<string, object>();
            var index = Expression.Parameter(typeof(int), "i");

            var expr = node.ParaExpression(variaveis, contexto, index);

            var lambda = Expression.Lambda<Func<object>>(
                Expression.Convert(expr, typeof(object))
            );

            return lambda.Compile()();
        }

        [Fact]
        public void Soma_DeveFuncionar()
        {
            var node = new BinarioExpression(
                new ValueExpression(10f),
                "+",
                new ValueExpression(5f)
            );

            var result = Executar(node);

            Assert.Equal(15f, result);
        }

        [Fact]
        public void Subtracao_DeveFuncionar()
        {
            var node = new BinarioExpression(
                new ValueExpression(10f),
                "-",
                new ValueExpression(3f)
            );

            var result = Executar(node);

            Assert.Equal(7f, result);
        }

        [Fact]
        public void Multiplicacao_DeveFuncionar()
        {
            var node = new BinarioExpression(
                new ValueExpression(2f),
                "*",
                new ValueExpression(4f)
            );

            var result = Executar(node);

            Assert.Equal(8f, result);
        }

        [Fact]
        public void Divisao_DeveFuncionar()
        {
            var node = new BinarioExpression(
                new ValueExpression(10f),
                "/",
                new ValueExpression(2f)
            );

            var result = Executar(node);

            Assert.Equal(5f, result);
        }

        [Fact]
        public void Comparacao_Maior_DeveFuncionar()
        {
            var node = new BinarioExpression(
                new ValueExpression(10f),
                ">",
                new ValueExpression(5f)
            );

            var result = Executar(node);

            Assert.Equal(true, result);
        }

        [Fact]
        public void Comparacao_Menor_DeveFuncionar()
        {
            var node = new BinarioExpression(
                new ValueExpression(2f),
                "<",
                new ValueExpression(5f)
            );

            var result = Executar(node);

            Assert.Equal(true, result);
        }

        [Fact]
        public void Igualdade_DeveFuncionar()
        {
            var node = new BinarioExpression(
                new ValueExpression(5f),
                "==",
                new ValueExpression(5f)
            );

            var result = Executar(node);

            Assert.Equal(true, result);
        }

        [Fact]
        public void Logico_And_DeveFuncionar()
        {
            var node = new BinarioExpression(
                new ValueExpression(true),
                "&&",
                new ValueExpression(false)
            );

            var result = Executar(node);

            Assert.Equal(false, result);
        }

        [Fact]
        public void Logico_Or_DeveFuncionar()
        {
            var node = new BinarioExpression(
                new ValueExpression(true),
                "||",
                new ValueExpression(false)
            );

            var result = Executar(node);

            Assert.Equal(true, result);
        }

        [Fact]
        public void Nullable_Com_ValorNull_DeveNaoExplodir()
        {
            var node = new BinarioExpression(
                new ValueExpression((float?)null),
                "+",
                new ValueExpression(10f)
            );

            var result = Executar(node);

            Assert.Null(result);
        }
    }
}
