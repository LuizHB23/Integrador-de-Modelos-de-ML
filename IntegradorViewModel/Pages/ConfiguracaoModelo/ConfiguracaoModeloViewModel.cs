using CommunityToolkit.Mvvm.ComponentModel;
using IntegradorAplicacao.DTO;
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
        private DataView _schemaPreview;

        [ObservableProperty]
        private ModeloConfiguracao _modelo;

        public ObservableCollection<TransformadorDTO> Transformadores { get; }

        public Dictionary<string, string> Pipeline { get; set; }

        private IContext <ModeloDTO> _context;
        private IDialogService _dialogService;
        private ConversorJson _conversor;

        private ModeloDTO _modeloDTO;

        public ConfiguracaoModeloViewModel(INavigationService navigation, IDialogService dialogService, IContext<ModeloDTO> context, ConversorJson conversor)
        {
            Navigation = navigation;

            _context = context;
            _dialogService = dialogService;
            _conversor = conversor;

            var caminhoModeloJson = Path.Combine(Path.GetDirectoryName(_context.RecebeMensagem().CaminhoPasta!)!, "modelo.json");
            _modelo = _conversor.CarregarJson<ModeloConfiguracao>(caminhoModeloJson);

            _modeloDTO = new ModeloDTO(_modelo.NomeModelo, ParserTipoModelo.TipoModeloParaString(_modelo.Tipo), _modelo.CaminhoPasta, _modelo.Versao);
            _modelo.CaminhoPasta = Path.GetFileName(_modelo.CaminhoPasta);

            CaminhoArquivoDados = string.Empty;

            Pipeline = CarregarPipeline();
            SchemaPreview = CarregarSchema();
            Transformadores = CarregarTransformadores();
        }

        private DataView? CarregarSchema()
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Coluna", typeof(string));
            dataTable.Columns.Add("Finalidade", typeof(string));
            dataTable.Columns.Add("Tipo", typeof(string));
            dataTable.Columns.Add("Categorico", typeof(bool));

            string caminhoSchema = Path.Combine(Path.GetDirectoryName(_modeloDTO.CaminhoPasta)!, "schema.json");

            if (!File.Exists(caminhoSchema))
            {
                return dataTable.DefaultView;
            }

            var schema = _conversor.CarregarJson<Dictionary<int, SchemaDTO>>(caminhoSchema);

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

        private Dictionary<string, string> CarregarPipeline()
        {
            Dictionary<string, string> dicionarioPipeline = new();

            string caminhoPipeline = Path.Combine(Path.GetDirectoryName(_modeloDTO.CaminhoPasta)!, "pipeline.json");

            if (!File.Exists(caminhoPipeline))
            {
                return dicionarioPipeline;
            }

            var pipeline = _conversor.CarregarJson<Dictionary<int, FuncaoDTO>>(caminhoPipeline);

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

        private ObservableCollection<TransformadorDTO> CarregarTransformadores()
        {
            ObservableCollection<TransformadorDTO> listaTransformadores = new();

            string caminhoTransformadores = Path.Combine(Path.GetDirectoryName(_modeloDTO.CaminhoPasta)!, "transformador.json");

            if (!File.Exists(caminhoTransformadores))
            {
                return listaTransformadores;
            }

            var transformadores = _conversor.CarregarJson<Dictionary<int, TransformadorDTO>>(caminhoTransformadores);

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
