using AutoMapper;
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

        private readonly ModeloDTO _modelo;
        private readonly CardsConfigurarSchemaManager _cardsManager;


        private readonly IConversorJson _conversor;
        private readonly IContext<ModeloDTO> _contextModelo;
        private readonly IDialogService _dialogService;
        private readonly IMapper _mapper;

        public ConfigurarSchemaViewModel(INavigationService navigation, IDialogService dialogService, IConversorJson conversor, IContext<ModeloDTO> contextModelo, IMapper mapper)
        {
            _mapper = mapper;
            _conversor = conversor;
            _contextModelo = contextModelo;
            _dialogService = dialogService;
            Navigation = navigation;

            NomeColuna = string.Empty;
            Finalidade = string.Empty;
            Tipo = string.Empty;
            Categorico = false;
            CardsSchema = new();
            OpcoesPosicao = new();

            _modelo = _contextModelo.RecebeMensagem();
            _cardsManager = new(CardsSchema, OpcoesPosicao, _modelo);
        }

        [RelayCommand]
        public async Task AdicinarColuna()
        {
            if (string.IsNullOrWhiteSpace(NomeColuna) || string.IsNullOrWhiteSpace(Finalidade) || string.IsNullOrWhiteSpace(Tipo))
            {
                _dialogService.ShowMessage("Preencha corretamente os campos", "Campos Faltantes");
                return;
            }

            var posicao = CardsSchema.Count + 1;
            var schemaItem = new SchemaItemViewModel(posicao, NomeColuna, Finalidade, Tipo, Categorico);
            _cardsManager.AdicinarColuna(schemaItem, RemoverColuna, OrganizaPosicao);

            await PreparaParaJson();

            NomeColuna = string.Empty;
            Finalidade = string.Empty;
            Tipo = string.Empty;
            Categorico = false;
        }

        [RelayCommand]
        public async Task CarregarSchema()
        {
            await _cardsManager.CarregarSchema(_dialogService, _conversor, _mapper);
        }
        private async Task RemoverColuna(ConfiguracaoCardSchemaViewModel cardSchema)
        {
            await _cardsManager.RemoverCard(cardSchema);
            await PreparaParaJson();
        }
        private async Task OrganizaPosicao(ConfiguracaoCardSchemaViewModel cardSchema, int posicaoNova) => await _cardsManager.OrganizaPosicao(cardSchema, posicaoNova);
        private async Task PreparaParaJson() => await _cardsManager.PreparaParaJson(_conversor, _modelo.NomeModelo);


        [RelayCommand]
        public async Task NavigateToCarregarDados()
        {
            //Navigation.NavigateTo<CarregarDadosViewModel>();
            if (CardsSchema.Count == 0) 
            {
                _dialogService.ShowMessage("Não se pode criar um Schema vazio.", "Schema Vazio");
                return;
            }

            await Versionamento();

            Navigation.NavigateTo<CarregarDadosViewModel>();
        }

        [RelayCommand]
        public void NavigateToHome()
        {
            Navigation.EndFlow();
            Navigation.NavigateTo<HomeViewModel>();
        }

        private async Task Versionamento()
        {
            var nomeModelo = _contextModelo.RecebeMensagem().NomeModelo;

            var taskModelo = _conversor.CarregarJsonAsync<ModeloEmUsoConfiguracao>(nomeModelo);
            var taskSchema = _conversor.CarregarJsonAsync<SchemaConfiguracao>(nomeModelo);

            await Task.WhenAll(taskModelo, taskSchema);

            var modelo = await taskModelo;
            var schema = await taskSchema;

            modelo.PipelineVersao = "1.0";
            schema.Versao = "1.0";

            var listaPipeline = new List<SchemaConfiguracao>() { schema };

            var taskModeloJson = _conversor.ConverteJsonAsync(modelo, nomeModelo);
            var taskSchemaJson = _conversor.ConverteJsonAsync(listaPipeline, nomeModelo);

            await Task.WhenAll(taskModeloJson, taskSchemaJson);
        }
    }
}
