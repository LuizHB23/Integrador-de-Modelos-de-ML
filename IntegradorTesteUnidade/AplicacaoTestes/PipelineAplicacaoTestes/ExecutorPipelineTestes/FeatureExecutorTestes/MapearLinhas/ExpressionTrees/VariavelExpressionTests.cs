using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees;
using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.ExpressionsNo;
using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.Line;
using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.Variaveis;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.MapearLinhas.ExpressionTrees
{
    public class VariavelExpressionTests
    {
        private object Executar(List<NodeExpression> nodes, int linhas = 3)
        {
            var variaveis = new Dictionary<string, ParameterExpression>();
            var contexto = new Dictionary<string, object>();
            var index = Expression.Parameter(typeof(int), "i");

            var expressoes = new List<Expression>();

            foreach (var node in nodes)
            {
                expressoes.Add(node.ParaExpression(variaveis, contexto, index));
            }

            var block = Expression.Block(expressoes);

            var lambda = Expression.Lambda<Action<int>>(
                block,
                index
            ).Compile();

            // executa para cada linha (simulando DataFrame)
            for (int i = 0; i < linhas; i++)
            {
                lambda(i);
            }

            return contexto;
        }

        [Fact]
        public void Variavel_DeveArmazenarValor()
        {
            var nodes = new List<NodeExpression>
            {
                new VariavelExpression("x", new ValueExpression(10f), 3)
            };

            var contexto = (Dictionary<string, object>)Executar(nodes);

            dynamic varObj = contexto["x"];

            Assert.Equal(10f, varObj.PegarValor(0));
            Assert.Equal(10f, varObj.PegarValor(1));
            Assert.Equal(10f, varObj.PegarValor(2));
        }

        [Fact]
        public void Variavel_DevePermitirUsoPosterior()
        {
            var nodes = new List<NodeExpression>
            {
                new VariavelExpression("x", new ValueExpression(10f), 3),

                new VariavelExpression("y",
                    new BinarioExpression(
                        new VariavelUsoExpression("x"),
                        "+",
                        new ValueExpression(5f)
                    ),
                    3
                )
            };

            var contexto = (Dictionary<string, object>)Executar(nodes);

            dynamic y = contexto["y"];

            Assert.Equal(15f, y.PegarValor(0));
            Assert.Equal(15f, y.PegarValor(1));
            Assert.Equal(15f, y.PegarValor(2));
        }

        [Fact]
        public void Variavel_DeveSerAtualizadaPorLinha()
        {
            var nodes = new List<NodeExpression>
            {
                new VariavelExpression("x",
                    new BinarioExpression(
                        new ValueExpression(10f),
                        "+",
                        new ValueExpression(1f)
                    ),
                    3
                )
            };

            var contexto = (Dictionary<string, object>)Executar(nodes);

            dynamic x = contexto["x"];

            Assert.Equal(11f, x.PegarValor(0));
            Assert.Equal(11f, x.PegarValor(1));
            Assert.Equal(11f, x.PegarValor(2));
        }

        [Fact]
        public void Variavel_ComNull_DeveFuncionar()
        {
            var nodes = new List<NodeExpression>
            {
                new VariavelExpression("x", new ValueExpression(null), 3)
            };

            var contexto = (Dictionary<string, object>)Executar(nodes);

            dynamic x = contexto["x"];

            Assert.Null(x.PegarValor(0));
            Assert.Null(x.PegarValor(1));
            Assert.Null(x.PegarValor(2));
        }

        [Fact]
        public void Variavel_ComOperacaoComplexa_DeveFuncionar()
        {
            var nodes = new List<NodeExpression>
            {
                new VariavelExpression("a", new ValueExpression(10f), 3),

                new VariavelExpression("b",
                    new BinarioExpression(
                        new VariavelUsoExpression("a"),
                        "*",
                        new ValueExpression(2f)
                    ),
                    3
                ),

                new VariavelExpression("c",
                    new BinarioExpression(
                        new VariavelUsoExpression("b"),
                        "+",
                        new ValueExpression(5f)
                    ),
                    3
                )
            };

            var contexto = (Dictionary<string, object>)Executar(nodes);

            dynamic c = contexto["c"];

            Assert.Equal(25f, c.PegarValor(0));
            Assert.Equal(25f, c.PegarValor(1));
            Assert.Equal(25f, c.PegarValor(2));
        }

        [Fact]
        public void Variavel_Inexistente_DeveLancarErro()
        {
            var nodes = new List<NodeExpression>
            {
                new VariavelExpression("y",
                    new BinarioExpression(
                        new VariavelUsoExpression("x"),
                        "+",
                        new ValueExpression(5f)
                    ),
                    3
                )
            };

            Assert.Throws<Exception>(() => Executar(nodes));
        }
    }
}
