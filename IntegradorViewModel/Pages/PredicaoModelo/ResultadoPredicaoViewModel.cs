using CommunityToolkit.Mvvm.ComponentModel;
using IntegradorAplicacao.DTO;
using IntegradorDominio.DataFrameModel;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using System.Text;

namespace IntegradorViewModel.Pages.PredicaoModelo
{
    public partial class ResultadoPredicaoViewModel : ObservableObject
    {
        [ObservableProperty]
        private INavigationService _navigation;

        IContext<ArquivoDadosDTO> _contextArquivo;
        IContext<ModeloDTO> _contextModelo;

        private ArquivoDadosDTO _arquivo { get; set; }

        public ResultadoPredicaoViewModel(INavigationService navigation, IContext<ModeloDTO> contextModelo, IContext<ArquivoDadosDTO> contextArquivo)
        {
            Navigation = navigation;

            _contextArquivo = contextArquivo;
            _contextModelo = contextModelo;

            _arquivo = _contextArquivo.RecebeMensagem();

            var dataFrame = CarregarDataFrame();
        }
        private DataFrame CarregarDataFrame()
        {
            var linhas = File.ReadAllLines(_arquivo.CaminhoArquivoDados);
            var estadoCabecalho = ParseCsvLine(linhas[0]);

            var estadoColuna = estadoCabecalho.Select(_ => new List<string>()).ToArray();

            for (int i = 1; i < linhas.Length; i++)
            {
                var partes = ParseCsvLine(linhas[i]);

                for (int j = 0; j < estadoColuna.Length; j++)
                {
                    if (j < partes.Length)
                        estadoColuna[j].Add(partes[j]);
                    else
                        estadoColuna[j].Add(string.Empty);
                }
            }

            var dataFrame = new DataFrame();

            for (int i = 0; i < estadoCabecalho.Length; i++)
            {
                var dado = estadoColuna[i];
                dataFrame.AdicionarColuna(estadoCabecalho[i], dado);
            }

            return dataFrame;
        }

        // Função simples para parsear CSV com aspas
        private string[] ParseCsvLine(string linha)
        {
            var resultado = new List<string>();
            bool dentroAspas = false;
            var buffer = new StringBuilder();

            foreach (char c in linha)
            {
                if (c == '"')
                {
                    dentroAspas = !dentroAspas; // alterna estado
                    continue; // remove as aspas
                }

                if (c == ',' && !dentroAspas)
                {
                    resultado.Add(buffer.ToString());
                    buffer.Clear();
                }
                else
                {
                    buffer.Append(c);
                }
            }

            resultado.Add(buffer.ToString()); // adiciona último campo
            return resultado.ToArray();
        }
    }
}




