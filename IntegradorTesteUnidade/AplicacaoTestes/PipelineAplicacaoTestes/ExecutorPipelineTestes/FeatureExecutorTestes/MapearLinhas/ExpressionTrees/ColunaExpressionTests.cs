using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees;
using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.Line;
using IntegradorDominio.DataFrameModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.MapearLinhas.ExpressionTrees
{
    public class ColunaExpressionTests
    {
        private object Executar(ColunaExpression node, DataFrame df, int indexValue)
        {
            var variaveis = new Dictionary<string, ParameterExpression>();
            var contexto = new Dictionary<string, object>
            {
                { df.NomeContexto, df }
            };

            var index = Expression.Parameter(typeof(int), "i");

            var expr = node.ParaExpression(variaveis, contexto, index);

            var lambda = Expression.Lambda<Func<int, object>>(
                Expression.Convert(expr, typeof(object)),
                index
            ).Compile();

            return lambda(indexValue);
        }

        private DataFrame CriarDataFrame()
        {
            var df = new DataFrame { NomeContexto = "df" };

            df.AdicionarColuna("GastoTotal", new List<float?> { 100f, 200f, 300f });
            df.AdicionarColuna("Quantidade", new List<float?> { 1, 2, 3 });

            return df;
        }

        [Fact]
        public void Coluna_DeveRetornarValorCorreto_Float()
        {
            //Arrange
            var df = CriarDataFrame();

            var node = new ColunaExpression("df", "GastoTotal", typeof(float?));

            //Act
            var result = Executar(node, df, 1);

            //Assert
            Assert.Equal(200f, result);
        }

        [Fact]
        public void Coluna_DeveRetornarValorCorreto_IntConvertidoParaNullable()
        {
            //Arrange
            var df = CriarDataFrame();

            var node = new ColunaExpression("df", "Quantidade", typeof(float?));

            //Act
            var result = Executar(node, df, 2);

            //Assert
            Assert.Equal(3f, result);
        }

        [Fact]
        public void Coluna_DeveFuncionarComNull()
        {
            //Arrange
            var df = new DataFrame { NomeContexto = "df" };

            df.AdicionarColuna("Valor", new List<float?> { null, 10f, 20f });

            var node = new ColunaExpression("df", "Valor", typeof(float?));

            //Act
            var result = Executar(node, df, 0);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public void Coluna_DataFrameInexistente_DeveLancarErro()
        {
            //Arrange
            var df = CriarDataFrame();

            //Act
            var node = new ColunaExpression("df_invalido", "GastoTotal", typeof(float?));

            //Assert
            Assert.Throws<Exception>(() => Executar(node, df, 0));
        }

        [Fact]
        public void Coluna_ColunaInexistente_DeveRetornarErro()
        {
            //Arrange
            var df = CriarDataFrame();

            var node = new ColunaExpression("df", "ColunaFake", typeof(float?));

            //Assert
            Assert.ThrowsAny<Exception>(() => Executar(node, df, 0));
        }

        [Fact]
        public void Coluna_DeveFuncionarParaMultiplasLinhas()
        {
            //Arrange
            var df = CriarDataFrame();

            //Act
            var node = new ColunaExpression("df", "GastoTotal", typeof(float?));

            //Assert
            Assert.Equal(100f, Executar(node, df, 0));
            Assert.Equal(200f, Executar(node, df, 1));
            Assert.Equal(300f, Executar(node, df, 2));
        }

        [Fact]
        public void Coluna_DeveIntegrarComExpressaoBinaria()
        {
            //Arrange
            var df = CriarDataFrame();

            var variaveis = new Dictionary<string, ParameterExpression>();
            var contexto = new Dictionary<string, object>
            {
                { df.NomeContexto, df }
            };

            var index = Expression.Parameter(typeof(int), "i");

            var coluna = new ColunaExpression("df", "GastoTotal", typeof(float?));

            var binario = new BinarioExpression(
                coluna,
                "+",
                new ValueExpression(10f)
            );

            var expr = binario.ParaExpression(variaveis, contexto, index);

            var lambda = Expression.Lambda<Func<int, object>>(
                Expression.Convert(expr, typeof(object)),
                index
            ).Compile();

            //Act
            var result = lambda(1);

            //Assert
            Assert.Equal(210f, result);
        }
    }
}
