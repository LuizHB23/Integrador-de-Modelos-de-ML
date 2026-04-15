using IntegradorAplicacao.ArquivosController.Csv;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.ArquivosController.Csv
{
    public class CsvControllerTests : IDisposable
    {
        private readonly List<string> _arquivosTemp = new();

        private string CriarCsvTemp(string conteudo)
        {
            var path = Path.GetTempFileName();
            File.WriteAllText(path, conteudo);

            _arquivosTemp.Add(path);

            return path;
        }

        public void Dispose()
        {
            foreach (var arquivo in _arquivosTemp)
            {
                try
                {
                    if (File.Exists(arquivo))
                        File.Delete(arquivo);
                }
                catch
                {
                    // opcional: ignorar erro de cleanup em teste
                }
            }

            _arquivosTemp.Clear();
        }

        [Fact]
        public async Task CarregarArquivoAsync_DeveCarregarCabecalhoEColunas()
        {
            // Arrange
            var csv = """
                Nome,Idade,Cidade
                Ana,20,Sao Paulo
                Bruno,30,Campinas
                """;

            var path = CriarCsvTemp(csv);
            var controller = new CsvController();

            // Act
            await controller.CarregarArquivoAsync(path);

            // Assert
            Assert.Equal(new[] { "Nome", "Idade", "Cidade" }, controller.Cabecalho);

            Assert.Equal("Ana", controller.Colunas[0][0]);
            Assert.Equal("20", controller.Colunas[1][0]);
            Assert.Equal("Sao Paulo", controller.Colunas[2][0]);
        }

        [Fact]
        public async Task CarregarArquivoAsync_DevePreencherColunasFaltantesComVazio()
        {
            // Arrange
            var csv = """
                A,B,C
                1,2
                3,4,5
            """;

            var path = CriarCsvTemp(csv);
            var controller = new CsvController();

            // Act
            await controller.CarregarArquivoAsync(path);

            // Assert
            Assert.Equal("", controller.Colunas[2][0]);
        }

        [Fact]
        public async Task CarregarArquivoAsync_DeveLidarComAspasEVirgulas()
        {
            // Arrange
            var csv = """
                Nome,Descricao
                Ana,"Dev, senior"
                Bruno,"Data ""Engineer"" "
                """;

            var path = CriarCsvTemp(csv);
            var controller = new CsvController();

            // Act
            await controller.CarregarArquivoAsync(path);

            // Assert
            Assert.Equal("Dev, senior", controller.Colunas[1][0]);
            Assert.Equal("Data \"Engineer\" ", controller.Colunas[1][1]);
        }

        [Fact]
        public async Task CarregarArquivoAsync_DeveLancarException_QuandoCsvVazio()
        {
            // Arrange
            var path = CriarCsvTemp("");
            var controller = new CsvController();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                controller.CarregarArquivoAsync(path)
            );
        }

        [Fact]
        public void EscreveArquivo_NaoDeveLancarExcecao()
        {
            // Arrange
            var controller = new CsvController();
            var dados = new List<int> { 1, 2, 3 };

            // Act + Assert
            Assert.Throws<NotImplementedException>(() =>
               controller.EscreveArquivo(dados)
           );

        }
    }
}
