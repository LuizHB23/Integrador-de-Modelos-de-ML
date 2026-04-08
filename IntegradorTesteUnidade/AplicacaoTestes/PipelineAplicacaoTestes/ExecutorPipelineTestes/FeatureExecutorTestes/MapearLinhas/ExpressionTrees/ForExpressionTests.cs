using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees;
using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.ExpressionsNo;
using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.For;
using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.Line;
using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.Variaveis;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.MapearLinhas.ExpressionTrees
{
    public class ForExpressionTests
    {
        private Dictionary<string, object> Executar(NodeExpression node)
        {
            var variaveis = new Dictionary<string, ParameterExpression>();
            var contexto = new Dictionary<string, object>();
            var index = Expression.Parameter(typeof(int), "i");

            var expr = node.ParaExpression(variaveis, contexto, index);

            var lambda = Expression.Lambda<Action<int>>(expr, index).Compile();

            lambda(0); // executa uma vez (loop interno cuida do resto)

            return contexto;
        }

        [Fact]
        public void For_DeveExecutarLoopSimples()
        {
            // for i = 0; i < 3; i++
            var forNode = new ForExpression(
                "i",
                new VariavelExpression("i", new ValueExpression(0f), 10),
                new BinarioExpression(
                    new VariavelUsoExpression("i"),
                    "<",
                    new ValueExpression(3f)
                ),
                new VariavelExpression(
                    "i",
                    new BinarioExpression(
                        new VariavelUsoExpression("i"),
                        "+",
                        new ValueExpression(1f)
                    ),
                    10
                )
            );

            // corpo: x = i
            forNode.Corpo.Add(
                new VariavelExpression(
                    "x",
                    new VariavelUsoExpression("i"),
                    10
                )
            );

            var contexto = Executar(forNode);

            dynamic x = contexto["x"];

            // última iteração: i = 2
            Assert.Equal(2, x.PegarValor(0));
        }

        [Fact]
        public void For_NaoDeveExecutar_SeCondicaoInicialFalse()
        {
            // for i = 10; i < 3; i++
            var forNode = new ForExpression(
                "i",
                new VariavelExpression("i", new ValueExpression(10f), 10),
                new BinarioExpression(
                    new VariavelUsoExpression("i"),
                    "<",
                    new ValueExpression(3f)
                ),
                new VariavelExpression(
                    "i",
                    new BinarioExpression(
                        new VariavelUsoExpression("i"),
                        "+",
                        new ValueExpression(1f)
                    ),
                    10
                )
            );

            forNode.Corpo.Add(
                new VariavelExpression("x", new ValueExpression(1f), 10)
            );

            var contexto = Executar(forNode);

            dynamic x = contexto["x"];

            // nunca executou → valor null
            Assert.Null(x.PegarValor(0));
        }

        [Fact]
        public void For_DeveExecutarMultiplasIteracoes()
        {
            // soma = 0; for i=0; i<3; i++ => soma += i
            var nodes = new List<NodeExpression>();

            nodes.Add(
                new VariavelExpression("soma", new ValueExpression(0f), 10)
            );

            var forNode = new ForExpression(
                "i",
                new VariavelExpression("i", new ValueExpression(0f), 10),
                new BinarioExpression(
                    new VariavelUsoExpression("i"),
                    "<",
                    new ValueExpression(3f)
                ),
                new VariavelExpression(
                    "i",
                    new BinarioExpression(
                        new VariavelUsoExpression("i"),
                        "+",
                        new ValueExpression(1f)
                    ),
                    10
                )
            );

            forNode.Corpo.Add(
                new VariavelExpression(
                    "soma",
                    new BinarioExpression(
                        new VariavelUsoExpression("soma"),
                        "+",
                        new VariavelUsoExpression("i")
                    ),
                    10
                )
            );

            nodes.Add(forNode);

            var contexto = ExecutarBloco(nodes);

            dynamic soma = contexto["soma"];

            // 0 + 1 + 2 = 3
            Assert.Equal(3, soma.PegarValor(0));
        }

        [Fact]
        public void For_DeveAtualizarVariavelLoop()
        {
            var forNode = new ForExpression(
                "i",
                new VariavelExpression("i", new ValueExpression(0f), 10),
                new BinarioExpression(
                    new VariavelUsoExpression("i"),
                    "<",
                    new ValueExpression(2f)
                ),
                new VariavelExpression(
                    "i",
                    new BinarioExpression(
                        new VariavelUsoExpression("i"),
                        "+",
                        new ValueExpression(1f)
                    ),
                    10
                )
            );

            var contexto = Executar(forNode);

            dynamic i = contexto["i"];

            // após loop: i == 2
            Assert.Equal(2, i.PegarValor(0));
        }

        [Fact]
        public void For_ComCorpoVazio_NaoDeveFalhar()
        {
            var forNode = new ForExpression(
                "i",
                new VariavelExpression("i", new ValueExpression(0f), 10),
                new BinarioExpression(
                    new VariavelUsoExpression("i"),
                    "<",
                    new ValueExpression(2f)
                ),
                new VariavelExpression(
                    "i",
                    new BinarioExpression(
                        new VariavelUsoExpression("i"),
                        "+",
                        new ValueExpression(1f)
                    ),
                    10
                )
            );

            var contexto = Executar(forNode);

            Assert.True(contexto.ContainsKey("i"));
        }

        // 🔧 helper para múltiplos nodes
        private Dictionary<string, object> ExecutarBloco(List<NodeExpression> nodes)
        {
            var variaveis = new Dictionary<string, ParameterExpression>();
            var contexto = new Dictionary<string, object>();
            var index = Expression.Parameter(typeof(int), "i");

            var expressoes = nodes
                .Select(n => n.ParaExpression(variaveis, contexto, index))
                .ToList();

            var block = Expression.Block(expressoes);

            var lambda = Expression.Lambda<Action<int>>(block, index).Compile();

            lambda(0);

            return contexto;
        }
    }
}
