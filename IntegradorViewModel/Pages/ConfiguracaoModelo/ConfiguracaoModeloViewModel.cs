using CommunityToolkit.Mvvm.ComponentModel;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorEnum;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorDominio.Models.Configuracao;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using System.Collections.ObjectModel;
using System.Data;

namespace IntegradorViewModel.Pages.ConfiguracaoModelo
{
    public partial class ConfiguracaoModeloViewModel : ObservableObject
    {
        [ObservableProperty]
        private INavigationService _navigation;

        [ObservableProperty]
        private string _caminhoArquivoDados;

        [ObservableProperty]
        private DataView? _schemaPreview;

        [ObservableProperty]
        private ModeloConfiguracao _modelo;

        [ObservableProperty]
        public ObservableCollection<TransformadorDTO> _transformadores;

        [ObservableProperty]
        public Dictionary<string, string> _pipeline;

        private IContext <ModeloDTO> _context;
        private IPathProvider _provider;
        private IConversorJson _conversor;

        private ModeloDTO _modeloDTO;

        public ConfiguracaoModeloViewModel(INavigationService navigation, IPathProvider provider, IContext<ModeloDTO> context, IConversorJson conversor)
        {
            Navigation = navigation;

            _context = context;
            _provider = provider;
            _conversor = conversor;
        }

        public async Task InicializarAsync()
        {
            var caminhoModeloJson = _provider.GetCaminhoModeloConfig(_context.RecebeMensagem().NomeModelo);
            Modelo = await _conversor.CarregarJsonAsync<ModeloConfiguracao>(caminhoModeloJson);

            _modeloDTO = new ModeloDTO(Modelo.NomeModelo, ParserTipoModelo.TipoModeloParaString(Modelo.Tipo), Modelo.CaminhoPasta, Modelo.Versao);
            Modelo.CaminhoPasta = Path.GetFileName(Modelo.CaminhoPasta);

            CaminhoArquivoDados = string.Empty;

            Pipeline = await CarregarPipeline();
            SchemaPreview = await CarregarSchema();
            Transformadores = await CarregarTransformadores();
        }

        private async Task<DataView?> CarregarSchema()
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Coluna", typeof(string));
            dataTable.Columns.Add("Finalidade", typeof(string));
            dataTable.Columns.Add("Tipo", typeof(string));
            dataTable.Columns.Add("Categorico", typeof(bool));

            string caminhoSchema = _provider.GetCaminhoSchemaConfig(_context.RecebeMensagem().NomeModelo);

            if (!File.Exists(caminhoSchema))
            {
                return dataTable.DefaultView;
            }

            var schema = await _conversor.CarregarJsonAsync<Dictionary<int, SchemaDTO>>(caminhoSchema);

            string nomeColuna;
            string finalidade;
            string tipo;
            bool categorico;

            foreach (var item in schema)
            {
                nomeColuna = item.Value.NomeColuna;
                finalidade = item.Value.Finalidade;
                tipo = item.Value.Tipo;
                categorico = item.Value.Categorico;

                dataTable.Rows.Add(nomeColuna, finalidade, tipo, categorico);
            }

            return dataTable.DefaultView;
        }

        private async Task<Dictionary<string, string>> CarregarPipeline()
        {
            Dictionary<string, string> dicionarioPipeline = new();

            string caminhoPipeline = _provider.GetCaminhoPipelineConfig(_context.RecebeMensagem().NomeModelo);

            if (!File.Exists(caminhoPipeline))
            {
                return dicionarioPipeline;
            }

            var pipeline = await _conversor.CarregarJsonAsync<Dictionary<int, FuncaoDTO>>(caminhoPipeline);

            string codigo = string.Empty;

            foreach (var item in pipeline)
            {
                codigo = $"{item.Value.NomeFuncao}()" + "\n{";

                foreach (var linha in item.Value.Codigo)
                {

                    codigo += $"\n{linha}\n";
                }
                codigo += "}";

                dicionarioPipeline.Add(item.Value.NomeFuncao, codigo);
            }

            return dicionarioPipeline;
        }

        private async Task<ObservableCollection<TransformadorDTO>> CarregarTransformadores()
        {
            ObservableCollection<TransformadorDTO> listaTransformadores = new();

            string caminhoTransformadores = _provider.GetCaminhoTransformadorConfig(_context.RecebeMensagem().NomeModelo);

            if (!File.Exists(caminhoTransformadores))
            {
                return listaTransformadores;
            }

            var transformadores = await _conversor.CarregarJsonAsync<Dictionary<int, TransformadorDTO>>(caminhoTransformadores);

            foreach (var item in transformadores)
            {
                var nome = item.Value.NomeTransformador;
                var arquivo = !string.IsNullOrEmpty(item.Value.CaminhoTransformador)
                              ? Path.GetFileName(item.Value.CaminhoTransformador)
                              : string.Empty;

                listaTransformadores.Add(new TransformadorDTO(nome, arquivo));
            }

            return listaTransformadores;
        }
    }
}
