using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorDominio.Models.Configuracao;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using IntegradorViewModel.Shared.Manager.GerenciadorCards;
using System.Collections.ObjectModel;

namespace IntegradorViewModel.Pages.InserirModelo
{
    public partial class ConfigurarSchemaViewModel : ObservableObject
    {
        [ObservableProperty]
        private INavigationService _navigation;

        [ObservableProperty]
        private string _nomeColuna;

        [ObservableProperty]
        private string _finalidade;

        [ObservableProperty]
        private string _tipo;

        [ObservableProperty]
        private bool _categorico;

        public ObservableCollection<int> OpcoesPosicao;
        public ObservableCollection<ConfiguracaoCardSchemaViewModel> CardsSchema { get; }

        private readonly string _nomeModelo;
        private readonly CardsConfigurarSchemaManager _cardsManager;


        private readonly ConversorJson _conversor;
        private readonly IContext<ModeloDTO> _contextModelo;
        private readonly IPathProvider _provider;
        private readonly IDialogService _dialogService;

        public ConfigurarSchemaViewModel(INavigationService navigation, IDialogService dialogService, ConversorJson conversor, IContext<ModeloDTO> contextModelo, IPathProvider provider)
        {
            _conversor = conversor;
            _contextModelo = contextModelo;
            _provider = provider;
            _dialogService = dialogService;
            Navigation = navigation;

            NomeColuna = string.Empty;
            Finalidade = string.Empty;
            Tipo = string.Empty;
            Categorico = false;
            CardsSchema = new();
            OpcoesPosicao = new();

            _nomeModelo = _contextModelo.RecebeMensagem().NomeModelo;
            _cardsManager = new(CardsSchema, OpcoesPosicao);
        }

        [RelayCommand]
        public void AdicinarColuna()
        {
            if (string.IsNullOrWhiteSpace(NomeColuna) || string.IsNullOrWhiteSpace(Finalidade) || string.IsNullOrWhiteSpace(Tipo))
            {
                _dialogService.ShowMessage("Preencha corretamente os campos", "Campos Faltantes");
                return;
            }

            var posicao = CardsSchema.Count + 1;
            var schemaItem = new SchemaItemViewModel(posicao, NomeColuna, Finalidade, Tipo, Categorico);
            _cardsManager.AdicinarColuna(schemaItem, RemoverColuna, OrganizaPosicao);

            PreparaParaJson();

            NomeColuna = string.Empty;
            Finalidade = string.Empty;
            Tipo = string.Empty;
            Categorico = false;
        }

        [RelayCommand]
        public void CarregarSchema() => _cardsManager.CarregarSchema(_dialogService, _conversor);
        private void RemoverColuna(ConfiguracaoCardSchemaViewModel cardSchema)
        {
            _cardsManager.RemoverCard(cardSchema);
            PreparaParaJson();
        }
        private void OrganizaPosicao(ConfiguracaoCardSchemaViewModel cardSchema, int posicaoNova) => _cardsManager.OrganizaPosicao(cardSchema, posicaoNova);
        private void PreparaParaJson() => _cardsManager.PreparaParaJson(_conversor, _nomeModelo);


        [RelayCommand]
        public async Task NavigateToCarregarDados()
        {
            Navigation.NavigateTo<CarregarDadosViewModel>();
            if (CardsSchema.Count == 0) 
            {
                _dialogService.ShowMessage("Não se pode criar um Schema vazio.", "Schema Vazio");
                return;
            }

            PreparaParaJson();

            var caminhoModelo = Path.Combine(Path.GetDirectoryName(_contextModelo.RecebeMensagem().CaminhoPasta)! , "modelo.json");
            var modelo = await _conversor.CarregarJsonAsync<ModeloConfiguracao>(caminhoModelo); 
            modelo.PipelineVersao = "1.0";
            await _conversor.ConverteJsonAsync(modelo);

            Navigation.NavigateTo<CarregarDadosViewModel>();
        }

        [RelayCommand]
        public void NavigateToHome()
        {
            Navigation.EndFlow();
            Navigation.NavigateTo<HomeViewModel>();
        }
    }
}
