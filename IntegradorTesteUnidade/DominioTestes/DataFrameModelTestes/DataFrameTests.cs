using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorTesteUnidade.DominioTestes.DataFrameModelTestes
{
    public class DataFrameTests
    {
        [Fact]
        public void AdicionarColuna_DeveAdicionarComSucesso_QuandoTamanhosSaoIguais()
        {
            // Arrange
            var df = new DataFrame();
            var dados1 = new List<float?> { 1, 2, 3 };
            var dados2 = new List<string?> { "A", "B", "C" };

            // Act
            df.AdicionarColuna("Id", dados1);
            df.AdicionarColuna("Nome", dados2);

            // Assert
            Assert.Equal(2, df.Colunas.Count);
            Assert.Equal(3, df.QuantidadeLinhas);
            Assert.Equal(1, df.PegarColuna<float?>("Id").Dados[0]);
        }

        [Fact]
        public void AdicionarColuna_DeveLancarExcecao_QuandoTamanhoForDiferente()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Id", new List<int?> { 1, 2 });

            // Act & Assert
            var dadosInvalidos = new List<string?> { "A", "B", "C" }; // Tamanho 3 vs 2
            Assert.Throws<Exception>(() => df.AdicionarColuna("Erro", dadosInvalidos));
        }

        [Fact]
        public void RenomearColunas_DeveAtualizarDicionarioEPropriedadeNome()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Antigo", new List<int?> { 1 });

            // Act
            df.RenomearColunas("Antigo", "Novo");

            // Assert
            Assert.Null(df.PegarColunaBase("Antigo"));
            Assert.NotNull(df.PegarColunaBase("Novo"));
            Assert.Equal("Novo", df.PegarColunaBase("Novo").Nome);
        }

        [Fact]
        public void AlterarColuna_DeveMudarTipoDaColuna_UsandoReflection()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Valor", new List<string?> { "10", "20" });

            // Act - Alterando de string para int via método com Type
            var novosValores = new List<object?> { "100", "200" };
            df.AlterarColuna("Valor", novosValores, typeof(int));

            // Assert
            var colunaInt = df.PegarColuna<int>("Valor");
            Assert.NotNull(colunaInt);
            Assert.IsType<int>(colunaInt.Dados[0]);
            Assert.Equal(100, colunaInt.Dados[0]);
        }

        [Fact]
        public void PegarColuna_DeveRetornarNull_SeOTipoForIncorreto()
        {
            // Arrange
            var df = new DataFrame();
            df.AdicionarColuna("Id", new List<float?> { 1 });

            // Act
            var colunaComoString = df.PegarColuna<string>("Id");

            // Assert
            Assert.Null(colunaComoString);
        }
    }
}