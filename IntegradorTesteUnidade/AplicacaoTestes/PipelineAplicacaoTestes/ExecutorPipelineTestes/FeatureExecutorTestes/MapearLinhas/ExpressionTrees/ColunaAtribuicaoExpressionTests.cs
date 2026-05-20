using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.Line;
using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees;

using System.Linq.Expressions;
using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.ExpressionsNo;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.MapearLinhas.ExpressionTrees
{
    public class ColunaAtribuicaoExpressionTests
    {
        private (Dictionary<string, object> contexto, DataFrame df) CriarContextoComDataFrame(string nomeColuna)
        {
            List<float?> linhas = new() { 1f, 2f, 3f, 4f, 5f};
            var df = new DataFrame();
            df.AdicionarColuna(nomeColuna, linhas);

            var contexto = new Dictionary<string, object>
            {
                { "df", df }
            };

            return (contexto, df);
        }

        private void Executar(NodeExpression node, Dictionary<string, object> contexto, int linhas = 3)
        {
            var variaveis = new Dictionary<string, ParameterExpression>();
            var index = Expression.Parameter(typeof(int), "i");

            var expr = node.ParaExpression(variaveis, contexto, index);
            var lambda = Expression.Lambda<Action<int>>(expr, index).Compile();

            for (int i = 0; i < linhas; i++)
            {
                lambda(i);
            }
        }

        [Fact]
        public void Deve_Atribuir_Valor_Constante_Na_Coluna()
        {
            var (contexto, df) = CriarContextoComDataFrame("valor");

            var node = new ColunaAtribuicaoExpression(
                "df",
                "valor",
                new ValueExpression(10f),
                typeof(float?)
            );

            Executar(node, contexto);

            var coluna = df.PegarColuna<float?>("valor");

            Assert.Equal(10f, coluna.Get(0));
            Assert.Equal(10f, coluna.Get(1));
            Assert.Equal(10f, coluna.Get(2));
        }

        [Fact]
        public void Deve_Atribuir_Resultado_De_Operacao()
        {
            var (contexto, df) = CriarContextoComDataFrame("valor");

            var node = new ColunaAtribuicaoExpression(
                "df",
                "valor",
                new BinarioExpression(
                    new ValueExpression(5f),
                    "+",
                    new ValueExpression(5f)
                ),
                typeof(float?)
            );

            Executar(node, contexto);

            var coluna = df.PegarColuna<float?>("valor");

            Assert.Equal(10f, coluna.Get(0));
        }

        [Fact]
        public void Deve_Atribuir_Null_Quando_Valor_For_Null()
        {
            var (contexto, df) = CriarContextoComDataFrame("valor");

            var node = new ColunaAtribuicaoExpression(
                "df",
                "valor",
                new ValueExpression(null),
                typeof(float?)
            );

            Executar(node, contexto);

            var coluna = df.PegarColuna<float?>("valor");

            Assert.Null(coluna.Get(0));
        }

        [Fact]
        public void Deve_Lancar_Excecao_Se_DataFrame_Nao_Existir()
        {
            var contexto = new Dictionary<string, object>();

            var node = new ColunaAtribuicaoExpression(
                "df",
                "valor",
                new ValueExpression(10f),
                typeof(float?)
            );

            var variaveis = new Dictionary<string, ParameterExpression>();
            var index = Expression.Parameter(typeof(int), "i");

            Assert.Throws<Exception>(() =>
                node.ParaExpression(variaveis, contexto, index)
            );
        }

        [Fact]
        public void Deve_Atribuir_Para_Multiplas_Linhas()
        {
            var (contexto, df) = CriarContextoComDataFrame("valor");

            var node = new ColunaAtribuicaoExpression(
                "df",
                "valor",
                new ValueExpression(7f),
                typeof(float?)
            );

            Executar(node, contexto, 5);

            var coluna = df.PegarColuna<float?>("valor");

            for (int i = 0; i < 5; i++)
            {
                Assert.Equal(7f, coluna.Get(i));
            }
        }
    }
}
