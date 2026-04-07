using IntegradorAplicacao.PipelineAplicacao.ParserPipeline;
using IntegradorDominio.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ParserPipelineTestes
{
    public class ParserAstTests
    {
        private readonly ParserAst _parser = new ParserAst();

        [Fact]
        public void ParserCorpo_DeveExtrairNomeEMetodo()
        {
            string codigo = @"
                MetodoTeste(a, b)
                {
                    x = a + b
                    return x
                }";

            var resultado = _parser.ParserCorpo(codigo);

            Assert.Single(resultado);
            Assert.True(resultado.ContainsKey("MetodoTeste"));
            Assert.Equal(2, resultado["MetodoTeste"].Count);
            Assert.Contains("x = a + b", resultado["MetodoTeste"]);
            Assert.Contains("return x", resultado["MetodoTeste"]);
        }

        [Fact]
        public void Parse_DeveCriarMetodoPipelineComComandos()
        {
            var corpo = new Dictionary<string, List<string>>
            {
                { "Soma", new List<string> { "resultado = a + b", "return resultado" } }
            };

            var metodo = _parser.Parse(corpo);

            Assert.Equal("Soma", metodo.Nome);
            Assert.Equal(2, metodo.Comandos.Count);

            Assert.IsType<AtribuicaoMetodoPipeline>(metodo.Comandos[0]);
            Assert.IsType<RetornoMetodoPipeline>(metodo.Comandos[1]);
        }

        [Fact]
        public void ParseLinha_DeveReconhecerReturn()
        {
            var metodo = _parser.Parse(new Dictionary<string, List<string>>
            {
                { "TesteReturn", new List<string> { "return valor" } }
            });

            Assert.Single(metodo.Comandos);
            var comando = metodo.Comandos[0];
            Assert.IsType<RetornoMetodoPipeline>(comando);
            Assert.Equal("valor", ((RetornoMetodoPipeline)comando).Variavel);
        }

        [Fact]
        public void ParseLinha_DeveReconhecerAtribuicaoSimples()
        {
            var metodo = _parser.Parse(new Dictionary<string, List<string>>
            {
                { "TesteAtrib", new List<string> { "x = y" } }
            });

            Assert.Single(metodo.Comandos);
            var comando = metodo.Comandos[0];
            Assert.IsType<AtribuicaoMetodoPipeline>(comando);
            var atrib = (AtribuicaoMetodoPipeline)comando;

            Assert.Equal("x", atrib.Variavel);
            Assert.Equal("y", atrib.ChamadaMetodo.ObjetoInicial);
            Assert.Empty(atrib.ChamadaMetodo.Metodos);
        }

        [Fact]
        public void ParseLinha_DeveReconhecerChamadaMetodoComArgumentos()
        {
            var metodo = _parser.Parse(new Dictionary<string, List<string>>
            {
                { "TesteChain", new List<string> { "x = df.Filtro(coluna=\"idade\", valor=30).Somar()" } }
            });

            var comando = (AtribuicaoMetodoPipeline)metodo.Comandos[0];

            Assert.Equal("x", comando.Variavel);
            var chamada = comando.ChamadaMetodo;

            Assert.Equal("df", chamada.ObjetoInicial);
            Assert.Equal(2, chamada.Metodos.Count);

            var filtro = chamada.Metodos[0];
            Assert.Equal("Filtro", filtro.Nome);
            Assert.Equal(2, filtro.Argumentos.Count);
            Assert.Equal("coluna", filtro.Argumentos[0].Nome);
            Assert.Equal("idade", filtro.Argumentos[0].Valor);
            Assert.Equal("valor", filtro.Argumentos[1].Nome);
            Assert.Equal("30", filtro.Argumentos[1].Valor);

            var somar = chamada.Metodos[1];
            Assert.Equal("Somar", somar.Nome);
            Assert.Empty(somar.Argumentos);
        }

        [Fact]
        public void SplitInteligente_DeveTratarVirgulasDentroDeAspasEColchetes()
        {
            var argumentos = "a=1, b=\"texto, com virgula\", c=[1,2,3]";
            var metodo = _parser.Parse(new Dictionary<string, List<string>>
            {
                { "TesteSplit", new List<string> { "x = df.Metodo(" + argumentos + ")" } }
            });

            var chamada = ((AtribuicaoMetodoPipeline)metodo.Comandos[0]).ChamadaMetodo;
            var metodoChain = chamada.Metodos[0];

            Assert.Equal(3, metodoChain.Argumentos.Count);
            Assert.Equal("1", metodoChain.Argumentos[0].Valor);
            Assert.Equal("texto, com virgula", metodoChain.Argumentos[1].Valor);
            Assert.Equal("[1,2,3]", metodoChain.Argumentos[2].Valor);
        }

        [Fact]
        public void IndexOfAtribuicao_DeveIgnorarComparadores()
        {
            var parserPrivado = typeof(ParserAst).GetMethod("IndexOfAtribuicao", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            int idx = (int)parserPrivado!.Invoke(_parser, new object[] { "x = y == 10" })!;
            Assert.Equal(2, idx); // só o '=' da atribuição é detectado
        }

        [Fact]
        public void SplitPorPontoMetodo_DeveIgnorarPontoDecimal()
        {
            var parserPrivado = typeof(ParserAst).GetMethod("SplitPorPontoMetodo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var input = "df.ColunaFloat(3.14).OutroMetodo()";
            var resultado = (List<string>)parserPrivado!.Invoke(_parser, new object[] { input })!;
            Assert.Equal(3, resultado.Count);
            Assert.Equal("df", resultado[0]);
            Assert.Equal("ColunaFloat(3.14)", resultado[1]);
            Assert.Equal("OutroMetodo()", resultado[2]);
        }
    }
}
