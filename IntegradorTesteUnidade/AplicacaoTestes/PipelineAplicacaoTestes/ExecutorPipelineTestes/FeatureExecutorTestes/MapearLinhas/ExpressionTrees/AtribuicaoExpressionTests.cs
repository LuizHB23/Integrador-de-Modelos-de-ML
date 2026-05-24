using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees;
using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.ExpressionsNo;
using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.Line;
using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.Variaveis;
using System.Linq.Expressions;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.MapearLinhas.ExpressionTrees
{
    public class AtribuicaoExpressionTests
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
        public void Atribuicao_DeveCriarVariavel()
        {
            var nodes = new List<NodeExpression>
            {
                new VariavelExpression("x", new ValueExpression(10f), 3)
            };

            var contexto = Executar(nodes);

            dynamic x = contexto["x"];

            Assert.Equal(10f, x.PegarValor(0));
            Assert.Equal(10f, x.PegarValor(1));
        }

        [Fact]
        public void Atribuicao_UsandoVariavel_DeveFuncionar()
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

            var contexto = Executar(nodes);

            dynamic y = contexto["y"];

            Assert.Equal(15f, y.PegarValor(0));
        }

        [Fact]
        public void Atribuicao_Reatribuicao_DeveFuncionar()
        {
            var nodes = new List<NodeExpression>
            {
                new VariavelExpression("x", new ValueExpression(10f), 3),

                new VariavelExpression("x",
                    new BinarioExpression(
                        new VariavelUsoExpression("x"),
                        "+",
                        new ValueExpression(5f)
                    ),
                    3
                )
            };

            var contexto = Executar(nodes);

            dynamic x = contexto["x"];

            Assert.Equal(15f, x.PegarValor(0));
        }

        [Fact]
        public void Atribuicao_ComExpressaoComplexa()
        {
            var nodes = new List<NodeExpression>
            {
                new VariavelExpression("x",
                    new BinarioExpression(
                        new ValueExpression(10f),
                        "*",
                        new ValueExpression(2f)
                    ),
                    3
                )
            };

            var contexto = Executar(nodes);

            dynamic x = contexto["x"];

            Assert.Equal(20f, x.PegarValor(0));
        }

        [Fact]
        public void Atribuicao_ComBoolean()
        {
            var nodes = new List<NodeExpression>
            {
                new VariavelExpression("flag",
                    new BinarioExpression(
                        new ValueExpression(10f),
                        ">",
                        new ValueExpression(5f)
                    ),
                    3
                )
            };

            var contexto = Executar(nodes);

            dynamic flag = contexto["flag"];

            Assert.Equal(true, flag.PegarValor(0));
        }

        [Fact]
        public void Atribuicao_ComNull()
        {
            var nodes = new List<NodeExpression>
            {
                new VariavelExpression("x", new ValueExpression(null), 3)
            };

            var contexto = Executar(nodes);

            dynamic x = contexto["x"];

            Assert.Null(x.PegarValor(0));
        }

        [Fact]
        public void Atribuicao_ComNullEmOperacao()
        {
            var nodes = new List<NodeExpression>
            {
                new VariavelExpression("x", new ValueExpression(null), 3),

                new VariavelExpression("y",
                    new BinarioExpression(
                        new VariavelUsoExpression("x"),
                        "+",
                        new ValueExpression(10f)
                    ),
                    3
                )
            };

            var contexto = Executar(nodes);

            dynamic y = contexto["y"];

            Assert.Null(y.PegarValor(0));
        }

        [Fact]
        public void Atribuicao_VariavelInexistente_DeveFalhar()
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
