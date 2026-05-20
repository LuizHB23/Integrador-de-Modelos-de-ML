using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Gerenciador;
using Moq;
using Xunit;
using System;
using System.IO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;

namespace IntegradorTesteUnidade.AplicacaoTestes.GerenciadorTestes
{
    // Adicionei IDisposable aqui
    public class ModeloGerenciadorTests : IDisposable
    {
        private readonly Mock<IPathProvider> _mockProvider;
        private readonly string _appFolder;
        private readonly string _nomeModelo = "Teste Qualquer Aqui";

        public ModeloGerenciadorTests()
        {
            _mockProvider = new Mock<IPathProvider>();

            // Centralizei a lógica de caminho no construtor
            _appFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Integrador", "Modelos");
        }

        [Fact]
        public void RetornaCaminhoDestinoEOkParaPathProviderVistadoQuandoChamadoSalvar()
        {
            // Arrange
            _mockProvider.Setup(f => f.GetCaminhoModelo()).Returns(_appFolder);

            var pastaEspecificaModelo = Path.Combine(_appFolder, _nomeModelo);
            string caminhoDestinoEsperado = Path.Combine(pastaEspecificaModelo, "win.ini");

            var modeloGerenciador = new ModeloGerenciador(_mockProvider.Object);
            var modelo = new ModeloDTO(_nomeModelo, "", @"C:\Windows\win.ini");

            // Act
            var caminhoGerado = modeloGerenciador.Salvar(modelo);

            // Assert
            _mockProvider.Verify(f => f.GetCaminhoModelo(), Times.Once());
            Assert.Equal(caminhoDestinoEsperado, caminhoGerado);
            Assert.True(File.Exists(caminhoGerado));
        }

        public void Dispose()
        {
            var pastaParaDeletar = Path.Combine(_appFolder, _nomeModelo);

            if (Directory.Exists(pastaParaDeletar))
                Directory.Delete(pastaParaDeletar, true);
        }
    }
}