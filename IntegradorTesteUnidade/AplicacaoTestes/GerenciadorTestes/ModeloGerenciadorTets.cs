using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Gerenciador;
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
            var pastaApp = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            _mockProvider.Setup(f => f.GetCaminhoModelo()).Returns(pastaApp);

            var modeloGerenciador = new ModeloGerenciador(_mockProvider.Object);

            var modelo = new ModeloDTO("", "", "");

            //Act
            modeloGerenciador.Salvar(modelo);

            //Assert
            _mockProvider.Verify(f => f.GetCaminhoModelo(), Times.Once());
        }

        [Fact]
        public void RetornaCaminhoDestinoQuandoChamadoSalvar()
        {
            //Arrange
            string nomeModelo = "Teste";

            var appFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            appFolder = Path.Combine(appFolder, "Integrador", "Modelos");
            _mockProvider.Setup(f => f.GetCaminhoModelo()).Returns(appFolder);

            appFolder = Path.Combine(appFolder, nomeModelo);
            string caminhoDestino = Path.Combine(appFolder);
            caminhoDestino = Path.Combine(caminhoDestino, "win.ini");

            var modeloGerenciador = new ModeloGerenciador(_mockProvider.Object);

            var modelo = new ModeloDTO(nomeModelo, "", @"C:\\Windows\\win.ini");

            //Act
            var caminhoModelo = modeloGerenciador.Salvar(modelo);

            //Assert
            try
            {
                Assert.Equal(caminhoDestino, caminhoModelo);
                Assert.True(File.Exists(caminhoModelo));
            }
            finally
            {
                if (File.Exists(caminhoDestino)) File.Delete(caminhoDestino);
                if (Directory.Exists(appFolder)) Directory.Delete(appFolder);
            }
        }
    }
}
