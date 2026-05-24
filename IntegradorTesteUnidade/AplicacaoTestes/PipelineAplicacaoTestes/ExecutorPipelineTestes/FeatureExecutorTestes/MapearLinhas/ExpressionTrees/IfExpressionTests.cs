using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees;
using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.ExpressionsNo;
using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.If;
using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.Line;
using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.Variaveis;
using System.Linq.Expressions;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.MapearLinhas.ExpressionTrees
{
    public class IfExpressionTests
    {
        private Dictionary<string, object> Executar(List<NodeExpression> nodes, int linhas = 3)
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

            var lambda = Expression.Lambda<Action<int>>(block, index).Compile();

            for (int i = 0; i < linhas; i++)
            {
                lambda(i);
            }

            return contexto;
        }

        [Fact]
        public void If_Simples_True_DeveExecutar()
        {
            var ifNode = new IfExpression(
                new BinarioExpression(
                    new ValueExpression(10f),
                    ">",
                    new ValueExpression(5f)
                )
            );

            ifNode.Body.Add(
                new VariavelExpression("x", new ValueExpression(100f), 3)
            );

            var contexto = Executar(new List<NodeExpression> { ifNode });

            dynamic x = contexto["x"];

            Assert.Equal(100f, x.PegarValor(0));
        }

        [Fact]
        public void If_Simples_False_NaoDeveExecutar()
        {
            var ifNode = new IfExpression(
                new BinarioExpression(
                    new ValueExpression(2f),
                    ">",
                    new ValueExpression(5f)
                )
            );

            ifNode.Body.Add(
                new VariavelExpression("x", new ValueExpression(100f), 3)
            );

            var contexto = Executar(new List<NodeExpression> { ifNode });

            if (contexto.ContainsKey("x"))
            {
                dynamic x = contexto["x"];
                Assert.Null(x.PegarValor(0));
            }
        }

        [Fact]
        public void If_Else_DeveExecutarElse()
        {
            var ifNode = new IfExpression(
                new BinarioExpression(
                    new ValueExpression(2f),
                    ">",
                    new ValueExpression(5f)
                )
            );

            ifNode.Body.Add(
                new VariavelExpression("x", new ValueExpression(100f), 3)
            );

            ifNode.ElseBody.Add(
                new VariavelExpression("x", new ValueExpression(50f), 3)
            );

            var contexto = Executar(new List<NodeExpression> { ifNode });

            dynamic x = contexto["x"];

            Assert.Equal(50f, x.PegarValor(0));
        }

        [Fact]
        public void If_Else_DeveExecutarIfQuandoTrue()
        {
            var ifNode = new IfExpression(
                new BinarioExpression(
                    new ValueExpression(10f),
                    ">",
                    new ValueExpression(5f)
                )
            );

            ifNode.Body.Add(
                new VariavelExpression("x", new ValueExpression(100f), 3)
            );

            ifNode.ElseBody.Add(
                new VariavelExpression("x", new ValueExpression(50f), 3)
            );

            var contexto = Executar(new List<NodeExpression> { ifNode });

            dynamic x = contexto["x"];

            Assert.Equal(100f, x.PegarValor(0));
        }

        [Fact]
        public void If_ComVariavel_DeveFuncionar()
        {
            var nodes = new List<NodeExpression>
            {
                new VariavelExpression("a", new ValueExpression(10f), 3)
            };

            var ifNode = new IfExpression(
                new BinarioExpression(
                    new VariavelUsoExpression("a"),
                    ">",
                    new ValueExpression(5f)
                )
            );

            ifNode.Body.Add(
                new VariavelExpression("b", new ValueExpression(999f), 3)
            );

            nodes.Add(ifNode);

            var contexto = Executar(nodes);

            dynamic b = contexto["b"];

            Assert.Equal(999f, b.PegarValor(0));
        }

        [Fact]
        public void If_Simples_False_NaoDeveAtribuirValor()
        {
            var ifNode = new IfExpression(
                new BinarioExpression(
                    new ValueExpression(2f),
                    ">",
                    new ValueExpression(5f)
                )
            );

            ifNode.Body.Add(
                new VariavelExpression("x", new ValueExpression(100f), 3)
            );

            var contexto = Executar(new List<NodeExpression> { ifNode });

            // variável pode existir, mas não deve ter valor
            if (contexto.ContainsKey("x"))
            {
                dynamic x = contexto["x"];
                Assert.Null(x.PegarValor(0));
            }
        }

        [Fact]
        public void If_MultiplasInstrucoes_NoBody()
        {
            var ifNode = new IfExpression(
                new ValueExpression(true)
            );

            ifNode.Body.Add(
                new VariavelExpression("a", new ValueExpression(10f), 3)
            );

            ifNode.Body.Add(
                new VariavelExpression("b",
                    new BinarioExpression(
                        new VariavelUsoExpression("a"),
                        "+",
                        new ValueExpression(5f)
                    ),
                    3
                )
            );

            var contexto = Executar(new List<NodeExpression> { ifNode });

            dynamic b = contexto["b"];

            Assert.Equal(15f, b.PegarValor(0));
        }

        [Fact]
        public void If_CondicaoNull_DeveFalhar()
        {
            var ifNode = new IfExpression(
                new ValueExpression(null)
            );

            ifNode.Body.Add(
                new VariavelExpression("x", new ValueExpression(1f), 3)
            );

            Assert.ThrowsAny<Exception>(() => Executar(new List<NodeExpression> { ifNode }));
        }
    }
}
