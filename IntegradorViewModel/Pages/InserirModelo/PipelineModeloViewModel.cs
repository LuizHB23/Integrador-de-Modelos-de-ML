using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorDominio.Pipeline.InterfacesSteps;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using IntegradorViewModel.Shared.Interfaces;
using IntegradorViewModel.Shared.Manager.GerenciadorCards;
using System.Collections.ObjectModel;
using System.Reflection;

namespace IntegradorViewModel.Pages.InserirModelo
{
    public partial class PipelineModeloViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _proximasFuncoes;

        [ObservableProperty]
        private INavigationService _navigation;

        [ObservableProperty]
        private ConfiguracaoMetodoTextBoxViewModel _textBox;

        private readonly IDialogService _dialogService;

        private readonly CardsPipelineModeloManager _cardsManager;

        public ObservableCollection<int> OpcoesPosicao;
        public ObservableCollection<ConfiguracaoCardFuncaoViewModel> CardsFuncoes { get; }
        public ObservableCollection<FeatureEngineeringItemViewModel> ListaFeatureEngineering { get; }
        public ObservableCollection<TransformDataViewItemViewModel> ListaTransformDataView { get; }

        public PipelineModeloViewModel(INavigationService navigation, IDialogService dialogService)
        {
            _navigation = navigation;
            ListaFeatureEngineering = new();
            ListaTransformDataView = new();
            CarregarListas();

            CardsFuncoes = new();
            OpcoesPosicao = new();

            _dialogService = dialogService;
            _cardsManager = new(CardsFuncoes, OpcoesPosicao);
            _textBox = new ConfiguracaoMetodoTextBoxViewModel(dialogService);
        }

        //Precisa de Manutção
        [RelayCommand]
        public void AdicionaFuncao()
        {
            var modeloNomeCorpo = TextBox.MandaCodigoMetodo();

            if (modeloNomeCorpo != null)
            {
                var modeloElementos = modeloNomeCorpo.First();
                var funcaoItem = new FuncaoItemViewModel(CardsFuncoes.Count + 1, modeloElementos.Key, modeloElementos.Value);
                _cardsManager.AdicinarColuna(funcaoItem, RemoverColuna, OrganizaPosicao);
            }
        }

        [RelayCommand]
        public void ListaFeature() => ProximasFuncoes = false;

        [RelayCommand]
        public void ListaTransform() => ProximasFuncoes = true;

        [RelayCommand]
        public void CarregarSchema() => _cardsManager.CarregarSchema();
        private void RemoverColuna(ConfiguracaoCardFuncaoViewModel cardSchema)
        {
            _cardsManager.RemoverColuna(cardSchema);
            //PreparaParaJson();
        }
        private void OrganizaPosicao(ConfiguracaoCardFuncaoViewModel cardSchema, int posicaoNova) => _cardsManager.OrganizaPosicao(cardSchema, posicaoNova);
        private void PreparaParaJson() => _cardsManager.PreparaParaJson();

        [RelayCommand]
        public void NavigateToHome()
        {
            Navigation.EndFlow();
            Navigation.NavigateTo<HomeViewModel>();
        }

        private void CarregarListas()
        {
            var assembly = typeof(IStepFeature).Assembly;

            var titulosPastas = new Dictionary<string, string>
                {
                    { "OperacoesAritmeticas", "Operações Aritméticas" },
                    { "OperacoesEstatisticas", "Operações Estatísticas" },
                    { "OperacoesExponenciais", "Operações Exponenciais" },
                    { "OperacoesEspeciais", "Operações Especiais" },
                    { "LimpezaDados", "Limpeza de Dados" },
                    { "AgrupamentoDados", "Agrupamento de Dados" }
                };

            foreach (var pasta in titulosPastas)
            {
                var operacoesDaPasta = assembly.GetTypes()
                    .Where(t => t.Namespace != null &&
                        t.Namespace.EndsWith(pasta.Key) &&
                        typeof(IStepFeature).IsAssignableFrom(t) &&
                        !t.IsInterface &&
                        !t.IsAbstract &&
                        t.IsPublic)
                    .Select(t => (IStepFeature)Activator.CreateInstance(t)!)
                    .ToList();

                if (operacoesDaPasta.Any())
                {
                    var listaProcessos = new ObservableCollection<IStepFeature>();

                    foreach(var classe in operacoesDaPasta)
                    {
                        listaProcessos.Add(classe);
                    }

                    var featureItem = new FeatureEngineeringItemViewModel(listaProcessos, pasta.Value);
                    ListaFeatureEngineering.Add(featureItem);
                }
            }
        }
    }
}
