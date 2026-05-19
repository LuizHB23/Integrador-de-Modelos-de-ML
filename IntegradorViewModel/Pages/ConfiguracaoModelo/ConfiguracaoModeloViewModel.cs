using CommunityToolkit.Mvvm.ComponentModel;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using System.Collections.ObjectModel;
using System.Data;
using System.Xml.Linq;

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

        public ObservableCollection<TransformadorDTO> Transformadores { get; }

        public Dictionary<string, string> Pipeline { get; set; }

        IConverteJson<Dictionary<int, TransformadorDTO>> _conversorTransformador;
        IConverteJson<Dictionary<int, FuncaoDTO>> _conversorPipeline;
        IConverteJson<Dictionary<int, SchemaDTO>> _conversorSchema;
        IContext <ModeloDTO> _context;
        IDialogService _dialogService;

        private ModeloDTO _modelo;
        public ConfiguracaoModeloViewModel(INavigationService navigation, IDialogService dialogService, IContext<ModeloDTO> context, IConverteJson<Dictionary<int, TransformadorDTO>> conversorTransformador, IConverteJson<Dictionary<int, FuncaoDTO>> conversorPipeline, IConverteJson<Dictionary<int, SchemaDTO>> conversorSchema)
        {
            Navigation = navigation;

            _context = context;
            _dialogService = dialogService;
            _conversorPipeline = conversorPipeline;
            _conversorSchema = conversorSchema;
            _conversorTransformador = conversorTransformador;

            _modelo = _context.RecebeMensagem();

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

            string caminhoSchema = Path.Combine(Path.GetDirectoryName(_modelo.CaminhoPasta)!, "schema.json");

            if (!File.Exists(caminhoSchema))
            {
                return dataTable.DefaultView;
            }

            var schema = _conversorSchema.CarregarJson(caminhoSchema);

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

            string caminhoPipeline = Path.Combine(Path.GetDirectoryName(_modelo.CaminhoPasta)!, "pipeline.json");

            if (!File.Exists(caminhoPipeline))
            {
                return dicionarioPipeline;
            }

            var pipeline = _conversorPipeline.CarregarJson(caminhoPipeline);

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

            string caminhoTransformadores = Path.Combine(Path.GetDirectoryName(_modelo.CaminhoPasta)!, "transformador.json");

            if (!File.Exists(caminhoTransformadores))
            {
                return listaTransformadores;
            }

            var transformadores = _conversorTransformador.CarregarJson(caminhoTransformadores);

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
