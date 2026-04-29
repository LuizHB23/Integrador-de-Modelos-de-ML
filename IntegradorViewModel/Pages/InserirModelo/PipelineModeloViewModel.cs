using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ControleUsuario.ConfiguracaoMetodo.EstadoDataFrame;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using IntegradorViewModel.Shared.Manager.GerenciadorScriptExecutor;
using System.Collections.ObjectModel;
using System.Data;

namespace IntegradorViewModel.Pages.InserirModelo
{
    public partial class PipelineModeloViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _proximasFuncoes;

        [ObservableProperty]
        private INavigationService _navigation;

        [ObservableProperty]
        private ConfiguracaoPipelineTextBoxViewModel _textBox;

        [ObservableProperty]
        private DataView? _dataPreview;

        public ObservableCollection<int> OpcoesPosicao;
        public ObservableCollection<ConfiguracaoCardFuncaoViewModel> CardsFuncoes { get; }
        public ObservableCollection<FeatureEngineeringItemViewModel> ListaFeatureEngineering { get; }
        public ObservableCollection<TransformDataViewItemViewModel> ListaTransformDataView { get; }

        private readonly ScriptExecutorPipelineModeloManager _scriptManager;

        public PipelineModeloViewModel(INavigationService navigation, IDialogService dialogService, IConverteJson<Dictionary<int, FuncaoDTO>> converter, IContext<ModeloDTO> contextModelo, IContext<ArquivoDadosDTO> contextArquivo, IPathProvider provider)
        {
            Navigation = navigation;
            ListaFeatureEngineering = new();
            ListaTransformDataView = new();
            CarregarListas();

            DataPreview = new();
            CardsFuncoes = new();
            OpcoesPosicao = new();
            TextBox = new ConfiguracaoPipelineTextBoxViewModel(new ConfiguracaoTextBoxViewModel(dialogService, AlterouTabela), DataPreview, new EstadoDataFrameViewModel(contextArquivo.RecebeMensagem()));

            _scriptManager = new(dialogService, converter, contextModelo, contextArquivo, provider, CardsFuncoes, OpcoesPosicao, TextBox);
        }

        [RelayCommand]
        public async Task AdicionaFuncao() => await _scriptManager.AdicionaFuncao();

        [RelayCommand]
        public async Task CarregarPipeline() => await _scriptManager.CarregarPipeline();

        [RelayCommand]
        public async Task AtualizaFuncao() => await _scriptManager.AtualizaFuncao();

        public void AlterouTabela(DataView dataView) => DataPreview = dataView;

        [RelayCommand]
        public void NavigateToTransformers()
        {
            Navigation.NavigateTo<TransformadoresModeloViewModel>();
        }

        [RelayCommand]
        public void NavigateToHome()
        {
            Navigation.EndFlow();
            Navigation.NavigateTo<HomeViewModel>();
        }

        private void CarregarListas()
        {
            var assembly = typeof(IFeature).Assembly;

            var features = assembly.GetTypes()
                .Where(t =>
                    typeof(IFeature).IsAssignableFrom(t) &&
                    !t.IsInterface &&
                    !t.IsAbstract &&
                    t.IsPublic)
                .Select(t => new
                {
                    Tipo = t,
                    Atributo = t.GetCustomAttributes(typeof(FeatureAttribute), false)
                                .Cast<FeatureAttribute>()
                                .FirstOrDefault()
                })
                .Where(x => x.Atributo != null)
                .ToList();

            var grupos = features.GroupBy(x => x.Atributo!.Categoria);

            foreach (var grupo in grupos)
            {
                var instancias = grupo
                    .Select(x => (IFeature)Activator.CreateInstance(x.Tipo)!)
                    .ToList();

                var listaProcessos = new ObservableCollection<IFeature>(instancias);

                var featureItem = new FeatureEngineeringItemViewModel(
                    listaProcessos,
                    grupo.Key,
                    RecebeListaPropriedades
                );

                ListaFeatureEngineering.Add(featureItem);
            }
        }

        private void RecebeListaPropriedades(string featureName, List<string> listaPropriedades) => TextBox.EscreveScript(featureName, listaPropriedades);
    }
}
