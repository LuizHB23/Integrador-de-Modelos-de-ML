using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorAplicacao;
using IntegradorDominio.AST;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorAplicacaoTestes
{
    public class BuilderExecutorTests
    {
        private BuilderExecutor CriarBuilderComDataFrame(out Dictionary<string, object?> objetos)
        {
            objetos = new Dictionary<string, object?>();

            var df = new DataFrame();
            df.NomeContexto = "df";

            objetos["df"] = df;

            return new BuilderExecutor(objetos);
        }

        [Fact]
        public void ConstroiMetodo_DeveLancarErro_SeVariavelRetornoNaoDeclarada()
        {
            var builder = CriarBuilderComDataFrame(out var objetos);

            var metodo = new MetodoPipeline("Nome Qualquer")
            {
                Comandos = new List<ComandoMetodoPipeline>
                {
                    new RetornoMetodoPipeline("naoExiste")
                }
            };

            var ex = Assert.Throws<Exception>(() => builder.ConstroiMetodo(metodo));

            Assert.Contains("não foi declarada", ex.Message);
        }

        [Fact]
        public void ConstroiAtribuicao_DeveLancarErro_SeObjetoInicialNaoExiste()
        {
            var builder = new BuilderExecutor(new Dictionary<string, object?>());

            var atribuicao = new AtribuicaoMetodoPipeline("novoDf", new ChamadaMetodoPipeline("inexistente")
            {
                Metodos = new List<MetodoChainPipeline>()
            });

            var ex = Assert.Throws<Exception>(() => builder.ConstroiAtribuicao(atribuicao));

            Assert.Contains("não foi declarada", ex.Message);
        }

        [Fact]
        public void ConstroiAtribuicao_DeveCriarVariavelNoDicionario()
        {
            var builder = CriarBuilderComDataFrame(out var objetos);

            var atribuicao = new AtribuicaoMetodoPipeline("novoDf", new ChamadaMetodoPipeline("df")
            {
                Metodos = new List<MetodoChainPipeline>()
            });

            builder.ConstroiAtribuicao(atribuicao);

            Assert.True(objetos.ContainsKey("novoDf"));
        }

        [Fact]
        public void ExecutarMetodo_DeveRetornarDataFrameCorreto()
        {
            var builder = CriarBuilderComDataFrame(out var objetos);

            // Fake atribuição sem métodos (pass-through)
            var atribuicao = new AtribuicaoMetodoPipeline("df2", new ChamadaMetodoPipeline("df")
            {
                Metodos = new List<MetodoChainPipeline>()
            });

            var metodo = new MetodoPipeline("Nome Qualquer")
            {
                Comandos = new List<ComandoMetodoPipeline>
                {
                    atribuicao,
                    new RetornoMetodoPipeline("df2")
                }
            };

            builder.ConstroiMetodo(metodo);

            var resultado = builder.ExecutarMetodo((DataFrame)objetos["df"]!);

            Assert.NotNull(resultado);
            Assert.Equal("df2", resultado.NomeContexto);
        }

    }
}
