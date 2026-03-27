using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorDominio.InterfacesSteps;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using IntegradorViewModel.Shared.Manager.GerenciadorCards;
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
        private ConfiguracaoMetodoTextBoxViewModel _textBox;

        [ObservableProperty]
        private DataView _dataPreview;

        public ObservableCollection<int> OpcoesPosicao;
        public ObservableCollection<ConfiguracaoCardFuncaoViewModel> CardsFuncoes { get; }
        public ObservableCollection<FeatureEngineeringItemViewModel> ListaFeatureEngineering { get; }
        public ObservableCollection<TransformDataViewItemViewModel> ListaTransformDataView { get; }

        private readonly string _nomeModelo;
        private readonly CardsPipelineModeloManager _cardsManager;

        private readonly IConverteJson<Dictionary<int, FuncaoDTO>> _converter;
        private readonly IDialogService _dialogService;
        private readonly IContext<ArquivoDadosDTO> _contextArquivo;
        private readonly IContext<ModeloDTO> _contextNomeModelo;

        public PipelineModeloViewModel(INavigationService navigation, IDialogService dialogService, IConverteJson<Dictionary<int, FuncaoDTO>> converter, IContext<ModeloDTO> contextNomeModelo, IContext<ArquivoDadosDTO> contextArquivo)
        {
            _converter = converter;
            _dialogService = dialogService;
            _navigation = navigation;
            _contextNomeModelo = contextNomeModelo;
            _contextArquivo = contextArquivo;

            ListaFeatureEngineering = new();
            ListaTransformDataView = new();
            CarregarListas();

            DataPreview = new();
            CardsFuncoes = new();
            OpcoesPosicao = new();
            TextBox = new ConfiguracaoMetodoTextBoxViewModel(dialogService, _contextArquivo.RecebeMensagem(), AlterouTabela, DataPreview);
            _nomeModelo = _contextNomeModelo.RecebeMensagem().NomeModelo;
            _cardsManager = new(CardsFuncoes, OpcoesPosicao);
        }

        //Precisa de Manutção
        [RelayCommand]
        public void AdicionaFuncao()
        {
            var modeloNomeCorpo = TextBox.MandaCodigoMetodo();

            if ((modeloNomeCorpo is null) || (modeloNomeCorpo.Count == 0))
            {
                return;
            }

            var modeloElementos = modeloNomeCorpo.First();
            var funcaoItem = new FuncaoItemViewModel(CardsFuncoes.Count + 1, modeloElementos.Key, modeloElementos.Value);
            _cardsManager.AdicinarColuna(funcaoItem, RemoverColuna, OrganizaPosicao);
            PreparaParaJson();
        }

        [RelayCommand]
        public void ListaFeature() => ProximasFuncoes = false;

        [RelayCommand]
        public void ListaTransform() => ProximasFuncoes = true;

        [RelayCommand]
        public void CarregarSchema() => _cardsManager.CarregarSchema(_dialogService, _converter);
        private void RemoverColuna(ConfiguracaoCardFuncaoViewModel cardSchema)
        {
            _cardsManager.RemoverColuna(cardSchema);
            PreparaParaJson();
        }
        private void OrganizaPosicao(ConfiguracaoCardFuncaoViewModel cardSchema, int posicaoNova)
        {
            _cardsManager.OrganizaPosicao(cardSchema, posicaoNova);
            PreparaParaJson();
        }

        private void PreparaParaJson() => _cardsManager.PreparaParaJson(_converter, _nomeModelo);

        public void AlterouTabela(DataView dataView)
        {
            DataPreview = dataView;
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
                        typeof(IFeature).IsAssignableFrom(t) &&
                        !t.IsInterface &&
                        !t.IsAbstract &&
                        t.IsPublic)
                    .Select(t => (IFeature)Activator.CreateInstance(t)!)
                    .ToList();

                if (operacoesDaPasta.Any())
                {
                    var listaProcessos = new ObservableCollection<IFeature>();

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
