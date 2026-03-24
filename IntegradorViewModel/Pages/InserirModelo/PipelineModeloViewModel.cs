using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorDominio.Pipeline.InterfacesSteps;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
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

        public ObservableCollection<int> OpcoesPosicao;
        public ObservableCollection<ConfiguracaoCardFuncaoViewModel> CardsFuncoes { get; }
        public ObservableCollection<FeatureEngineeringItemViewModel> ListaFeatureEngineering { get; }
        public ObservableCollection<TransformDataViewItemViewModel> ListaTransformDataView { get; }

        public PipelineModeloViewModel(INavigationService navigation)
        {
            _navigation = navigation;
            ListaFeatureEngineering = new();
            ListaTransformDataView = new();
            CarregarListas();

            CardsFuncoes = new();
            OpcoesPosicao = new();
        }

        //Precisa de Manutção
        [RelayCommand]
        public void AdicionaFuncao()
        {
            var funcaoItem = new FuncaoItemViewModel(1, "", "");
            CardsFuncoes.Add(new ConfiguracaoCardFuncaoViewModel(funcaoItem, RemoverColuna, OrganizaPosicao));
        }

        [RelayCommand]
        public void ListaFeature() => ProximasFuncoes = false;

        [RelayCommand]
        public void ListaTransform() => ProximasFuncoes = true;

        private void RemoverColuna(ConfiguracaoCardFuncaoViewModel cardFuncao)
        {
            CardsFuncoes.Remove(cardFuncao);
            OpcoesPosicao.Remove(CardsFuncoes.Count + 1);
            AtualizaPosicoes();
        }

        private void AtualizaPosicoes()
        {
            for (int i = 0; i < CardsFuncoes.Count; i++)
            {
                CardsFuncoes[i].EstouReposicionando = true;

                CardsFuncoes[i].OpcoesPosicao = OpcoesPosicao;

                CardsFuncoes[i].Posicao = i + 1;

                CardsFuncoes[i].EstouReposicionando = false;
            }
        }

        private void OrganizaPosicao(ConfiguracaoCardFuncaoViewModel cardFuncao, int posicaoNova)
        {
            int posicaoOriginal = CardsFuncoes.IndexOf(cardFuncao);

            CardsFuncoes.Move(posicaoOriginal, posicaoNova);

            AtualizaPosicoes();
        }

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
