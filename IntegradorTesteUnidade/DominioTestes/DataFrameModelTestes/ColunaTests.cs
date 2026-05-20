using IntegradorDominio.Models.DataFrameModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.DominioTestes.DataFrameModelTestes
{
    public class ColunaTests
    {
        [Fact]
        public void Construtor_DeveInicializarPropriedadesCorretamente()
        {
            // Arrange
            var nome = "Preco";
            var dados = new List<double?> { 10.5, 20.0, null };

            // Act
            var coluna = new Coluna<double?>(nome, dados);

            // Assert
            Assert.Equal(nome, coluna.Nome);
            Assert.Equal(3, coluna.Quantidade);
            Assert.Equal(typeof(double?), coluna.TipoDado);
        }

        [Fact]
        public void PegarValor_DeveRetornarValorCorreto_PeloIndice()
        {
            // Arrange
            var dados = new List<string?> { "A", "B", "C" };
            var coluna = new Coluna<string>("Letras", dados);

            // Act
            var valor = coluna.PegarValor(1); // "B"

            // Assert
            Assert.Equal("B", valor);
        }

        [Fact]
        public void AdicionaValor_DeveInserirObjeto_FazendoCastCorreto()
        {
            // Arrange
            var coluna = new Coluna<float?>("Idade", new List<float?> { 25 });
            object novoValor = 30.0f;

            // Act
            coluna.AdicionaValor(novoValor);

            // Assert
            Assert.Equal(2, coluna.Quantidade);
            Assert.Equal(30, coluna.Get(1));
        }

        [Fact]
        public void Clonar_DeveCriarNovaInstancia_ComMesmosDadosMasReferenciaDiferente()
        {
            // Arrange
            var listaOriginal = new List<float?> { 1, 2, 3 };
            var colunaOriginal = new Coluna<float?>("Original", listaOriginal);

            // Act
            var cloneBase = colunaOriginal.Clonar();
            var colunaClonada = cloneBase as Coluna<float?>;

            // Modifica o original para testar independência
            colunaOriginal.AdicionaValor(99.0f);

            // Assert
            Assert.NotNull(colunaClonada);
            Assert.Equal(3, colunaClonada.Quantidade); // O clone deve manter 3, original tem 4
            Assert.NotSame(colunaOriginal.Dados, colunaClonada.Dados);
            Assert.Equal(colunaOriginal.Nome, colunaClonada.Nome);
        }

        [Fact]
        public void InjetarValor_DeveSubstituirValorNoIndiceEspecifico()
        {
            // Arrange
            var coluna = new Coluna<bool?>("Ativo", new List<bool?> { true, true });
            object falso = false;

            // Act
            coluna.InjetarValor(0, falso);

            // Assert
            Assert.False(coluna.Get(0));
            Assert.True(coluna.Get(1));
        }
    }
}
