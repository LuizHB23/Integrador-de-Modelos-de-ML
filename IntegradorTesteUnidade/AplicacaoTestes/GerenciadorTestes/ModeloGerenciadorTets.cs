using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Gerenciador;
using IntegradorAplicacao.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.GerenciadorTestes
{
    public class ModeloGerenciadorTets
    {
        private readonly Mock<IPathProvider> _mockProvider;
        public ModeloGerenciadorTets()
        {
            _mockProvider = new Mock<IPathProvider>();
        }

        [Fact]
        public void RetornaOkParaPathProviderVistadoEmSalvar()
        {
            //Arrange
            var modeloGerenciador = new ModeloGerenciador(_mockProvider.Object);

            //Act
            modeloGerenciador.Salvar(It.IsAny<ModeloDTO>());

            //Assert
            _mockProvider.Verify(f => f.GetCaminhoModelo(), Times.Once());
        }
    }
}
