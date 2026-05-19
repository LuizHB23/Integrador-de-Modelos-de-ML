using CommunityToolkit.Mvvm.ComponentModel;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace IntegradorViewModel.Pages.ConfiguracaoModelo
{
    public partial class ConfiguracaoModeloViewModel : ObservableObject
    {
        [ObservableProperty]
        private INavigationService _navigation;

        [ObservableProperty]
        private string _caminhoArquivoDados;

        public Dictionary<string, string> Pipeline { get; set; }

        IConverteJson<Dictionary<int, FuncaoDTO>> _conversorSchema;
        IContext<ModeloDTO> _context;
        IDialogService _dialogService;

        private ModeloDTO _modelo;
        public ConfiguracaoModeloViewModel(INavigationService navigation, IDialogService dialogService, IContext<ModeloDTO> context, IConverteJson<Dictionary<int, FuncaoDTO>> conversorSchema)
        {
            Navigation = navigation;

            _context = context;
            _dialogService = dialogService;
            _conversorSchema = conversorSchema;

            _modelo = _context.RecebeMensagem();

            CaminhoArquivoDados = string.Empty;

            Pipeline = CarregarPipeline();
        }

        private Dictionary<string, string> CarregarPipeline()
        {
            Dictionary<string, string> dicionarioPipeline = new();

            string caminhoPipeline = Path.Combine(Path.GetDirectoryName(_modelo.CaminhoPasta)!, "pipeline.json");
            var pipeline =  _conversorSchema.CarregarJson(caminhoPipeline);

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

        //public void ConfigurarFuncao(ConfiguracaoCardFuncaoViewModel cardSchema)
        //{
        //    var caminhoPasta = _provider.GetCaminhoModelo();
        //    caminhoPasta = Path.Combine(caminhoPasta, _nomeModelo, _json);

        //    var dicionarioFuncoes = _converter.CarregarJson(caminhoPasta);
        //    var codigo = string.Empty;

        //    foreach (var elemento in dicionarioFuncoes)
        //    {
        //        if (elemento.Value.NomeFuncao == cardSchema.NomeMetodo)
        //        {
        //            codigo = $"{cardSchema.NomeMetodo}()" + "\n{";

        //            foreach (var linha in elemento.Value.Codigo)
        //            {

        //                codigo += $"\n{linha}\n";
        //            }
        //            codigo += "}";
        //        }
        //    }

        //    _textBox.ScriptCodigo = codigo;
        //}
    }
}
