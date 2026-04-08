using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.Parser;
using IntegradorDominio.FeatureEngineering.MapearLinhas.CasosLinha;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.MapearLinhas.Parser
{
    public class ParserMapTests
    {
        private readonly ParserMap _parser = new();

        [Fact]
        public void Parse_Line_DeveRetornarLineMap()
        {
            var resultado = _parser.Parse(@"[line:""x = 10""]");

            Assert.Single(resultado);
            var line = Assert.IsType<LineMap>(resultado[0]);

            Assert.Equal("x = 10", line.Linha);
        }

        [Fact]
        public void Parse_MultiplasLines_DeveRetornarListaCorreta()
        {
            var resultado = _parser.Parse(@"[line:""x = 10"", line:""y = x + 1""]");

            Assert.Equal(2, resultado.Count);

            Assert.Equal("x = 10", ((LineMap)resultado[0]).Linha);
            Assert.Equal("y = x + 1", ((LineMap)resultado[1]).Linha);
        }

        [Fact]
        public void Parse_IfSimples_DeveCriarIfMap()
        {
            var resultado = _parser.Parse(@"[if:{condition:""x > 10"", line:""x = x + 1""}]");

            var ifMap = Assert.IsType<IfMap>(resultado[0]);

            Assert.Equal("x > 10", ifMap.Condicao);
            Assert.Single(ifMap.Corpo);

            var line = Assert.IsType<LineMap>(ifMap.Corpo[0]);
            Assert.Equal("x = x + 1", line.Linha);

            Assert.Null(ifMap.Else);
        }

        [Fact]
        public void Parse_IfComElse_DeveCriarEstruturaCompleta()
        {
            var resultado = _parser.Parse(@"[if:{condition:""x > 10"", line:""x = x + 1"", else:{line:""x = 0""}}]");

            var ifMap = Assert.IsType<IfMap>(resultado[0]);

            Assert.Equal("x > 10", ifMap.Condicao);

            Assert.Single(ifMap.Corpo);
            Assert.Single(ifMap.Else);

            Assert.Equal("x = x + 1", ((LineMap)ifMap.Corpo[0]).Linha);
            Assert.Equal("x = 0", ((LineMap)ifMap.Else[0]).Linha);
        }

        [Fact]
        public void Parse_For_DeveCriarForMap()
        {
            var resultado = _parser.Parse(@"[for:{loop:""i = 0; i < 3; i = i+1"", line:""x = x + i""}]");

            var forMap = Assert.IsType<ForMap>(resultado[0]);

            Assert.Equal("i = 0; i < 3; i = i+1", forMap.Condicao);
            Assert.Single(forMap.Corpo);

            var line = Assert.IsType<LineMap>(forMap.Corpo[0]);
            Assert.Equal("x = x + i", line.Linha);
        }

        [Fact]
        public void Parse_EstruturaCompleta_DeveFuncionar()
        {
            var resultado = _parser.Parse(@"[line:""x = 1"", if:{condition:""x > 0"", line:""x = x + 1"", else:{line:""x = 0""}}, for:{loop:""i = 0; i < 2; i = i+1"", line:""x = x + i""}]");

            Assert.Equal(3, resultado.Count);

            Assert.IsType<LineMap>(resultado[0]);
            Assert.IsType<IfMap>(resultado[1]);
            Assert.IsType<ForMap>(resultado[2]);
        }

        [Fact]
        public void Parse_TrechoInvalido_DeveLancarExcecao()
        {
            Assert.Throws<Exception>(() =>
                _parser.Parse(@"[qualquercoisa:""x = 10""]")
            );
        }
    }
}
