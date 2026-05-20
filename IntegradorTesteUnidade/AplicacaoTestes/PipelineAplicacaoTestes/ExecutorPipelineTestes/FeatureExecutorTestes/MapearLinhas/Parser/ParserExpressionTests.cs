using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees;
using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.Line;
using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.Variaveis;
using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.Parser;
using IntegradorDominio.Models.DataFrameModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.MapearLinhas.Parser
{
    public class ParserExpressionTests
    {
        private readonly ParserExpression _parser = new();

        private DataFrame CriarDataFrameFake()
        {
            var df = new DataFrame();
            df.NomeContexto = "df";

            df.AdicionarColuna("GastoTotal", new List<float?> { 100f, 200f, 300f });
            df.AdicionarColuna("GastoMensal", new List<float?> { 10f, 20f, 30f });

            return df;
        }

        private Dictionary<string, object> CriarContexto(DataFrame df)
        {
            return new Dictionary<string, object>
            {
                { df.NomeContexto, df }
            };
        }

        [Fact]
        public void ParseLine_AtribuicaoColuna_DeveRetornarColunaAtribuicao()
        {
            var df = CriarDataFrameFake();
            var contexto = CriarContexto(df);

            var node = _parser.ParseLine(@"GastoTotal = 10", contexto, df);

            var result = Assert.IsType<ColunaAtribuicaoExpression>(node);
            Assert.Equal("GastoTotal", result.NomeColuna);
        }

        [Fact]
        public void ParseLine_AtribuicaoVariavel_DeveRetornarVariavel()
        {
            var df = CriarDataFrameFake();
            var contexto = CriarContexto(df);

            var node = _parser.ParseLine(@"x = 10", contexto, df);

            var result = Assert.IsType<VariavelExpression>(node);
            Assert.Equal("x", result.Nome);
        }

        [Fact]
        public void ParseExpression_Numero_DeveRetornarValue()
        {
            var expr = _parser.ParseExpression("10", new(), "df");

            var val = Assert.IsType<ValueExpression>(expr);
            Assert.Equal(10f, val.Valor);
        }

        [Fact]
        public void ParseExpression_String_DeveRetornarValue()
        {
            var expr = _parser.ParseExpression(@"""abc""", new(), "df");

            var val = Assert.IsType<ValueExpression>(expr);
            Assert.Equal("abc", val.Valor);
        }

        [Fact]
        public void ParseExpression_Coluna_DeveRetornarColunaExpression()
        {
            var df = CriarDataFrameFake();
            var contexto = CriarContexto(df);

            var expr = _parser.ParseExpression("GastoTotal", contexto, df.NomeContexto);

            Assert.IsType<ColunaExpression>(expr);
        }

        [Fact]
        public void ParseExpression_Variavel_DeveRetornarVariavelUso()
        {
            var expr = _parser.ParseExpression("x", new(), "df");

            Assert.IsType<VariavelUsoExpression>(expr);
        }

        [Fact]
        public void ParseExpression_Soma_DeveRetornarBinario()
        {
            var expr = _parser.ParseExpression("10 + 5", new(), "df");

            var bin = Assert.IsType<BinarioExpression>(expr);
            Assert.Equal("+", bin.Operador);
        }

        [Fact]
        public void ParseExpression_Precedencia_DeveRespeitarMultiplicacao()
        {
            var expr = _parser.ParseExpression("10 + 5 * 2", new(), "df");

            var bin = Assert.IsType<BinarioExpression>(expr);

            // raiz deve ser +
            Assert.Equal("+", bin.Operador);

            // direita deve ser *
            var direita = Assert.IsType<BinarioExpression>(bin.Right);
            Assert.Equal("*", direita.Operador);
        }

        [Fact]
        public void ParseExpression_Parenteses_DeveAlterarPrecedencia()
        {
            var expr = _parser.ParseExpression("(10 + 5) * 2", new(), "df");

            var bin = Assert.IsType<BinarioExpression>(expr);

            // raiz deve ser *
            Assert.Equal("*", bin.Operador);
        }

        [Fact]
        public void ParseExpression_Comparacao_DeveFuncionar()
        {
            var expr = _parser.ParseExpression("10 > 5", new(), "df");

            var bin = Assert.IsType<BinarioExpression>(expr);
            Assert.Equal(">", bin.Operador);
        }

        [Fact]
        public void ParseExpression_Booleano_DeveFuncionar()
        {
            var expr = _parser.ParseExpression("x > 5 && x < 10", new(), "df");

            var bin = Assert.IsType<BinarioExpression>(expr);
            Assert.Equal("&&", bin.Operador);
        }

        [Fact]
        public void ParseExpression_ExpressaoComplexa_DeveFuncionar()
        {
            var expr = _parser.ParseExpression("(x + 10) * 2 > 50 || x == 1", new(), "df");

            var bin = Assert.IsType<BinarioExpression>(expr);

            // operador raiz deve ser ||
            Assert.Equal("||", bin.Operador);
        }

        [Fact]
        public void ParseLine_ComLinePrefix_DeveFuncionar()
        {
            var df = CriarDataFrameFake();
            var contexto = CriarContexto(df);

            var node = _parser.ParseLine(@"line: GastoTotal = GastoTotal + 10", contexto, df);

            Assert.IsType<ColunaAtribuicaoExpression>(node);
        }

        [Fact]
        public void ParseLine_Invalido_DeveLancarErro()
        {
            var df = CriarDataFrameFake();
            var contexto = CriarContexto(df);

            Assert.Throws<System.Exception>(() =>
                _parser.ParseLine("GastoTotal 10", contexto, df)
            );
        }

        [Fact]
        public void ParseExpression_NumeroNegativo_DeveFuncionar()
        {
            var expr = _parser.ParseExpression("-10", new(), "df");

            var val = Assert.IsType<ValueExpression>(expr);
            Assert.Equal(-10f, val.Valor);
        }
    }
}
