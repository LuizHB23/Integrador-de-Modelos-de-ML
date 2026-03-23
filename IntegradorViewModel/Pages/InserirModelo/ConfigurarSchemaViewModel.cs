using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorViewModel.Context;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.Interfaces;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

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

        private readonly string _nomeModelo;

        public ObservableCollection<int> OpcoesPosicao;
        public ObservableCollection<ConfiguracaoCardSchemaViewModel> CardsSchema { get; }

        private readonly IConverteJson<Dictionary<int, SchemaDTO>> _converter;
        private readonly IContext<ModeloDTO> _contextNomeModelo;
        private readonly IPathProvider _provider;
        private readonly IDialogService _dialogService;

        public ConfigurarSchemaViewModel(INavigationService navigation, IDialogService dialogService, IConverteJson<Dictionary<int, SchemaDTO>> converter, IContext<ModeloDTO> contextNomeModelo, IPathProvider provider)
        {
            _converter = converter;
            _contextNomeModelo = contextNomeModelo;
            _provider = provider;
            _dialogService = dialogService;
            Navigation = navigation;

            _nomeModelo = _contextNomeModelo.RecebeMensagem().NomeModelo;
            NomeColuna = string.Empty;
            Finalidade = string.Empty;
            Tipo = string.Empty;
            Categorico = false;
            CardsSchema = new();
            OpcoesPosicao = new();
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
            var cardSchema = new ConfiguracaoCardSchemaViewModel(schemaItem, RemoverColuna, OrganizaPosicao);
            CardsSchema.Add(cardSchema);
            OpcoesPosicao.Add(posicao);

            AtualizaPosicoes();
            PreparaParaJson();

            NomeColuna = string.Empty;
            Finalidade = string.Empty;
            Tipo = string.Empty;
            Categorico = false;
        }

        [RelayCommand]
        public void CarregarSchema()
        {
            var _caminhoJson = _dialogService.GetCaminhoArquivo();

            if (string.IsNullOrWhiteSpace(_caminhoJson))
            {
                return;
            }

            CardsSchema.Clear();
            OpcoesPosicao.Clear();

            var schema = _converter.CarregarJson(_caminhoJson);

            foreach (var card in schema)
            {
                var schemaItem = new SchemaItemViewModel(card.Key, card.Value.NomeColuna, card.Value.Finalidade, card.Value.Tipo, card.Value.Categorico);
                var cardSchema = new ConfiguracaoCardSchemaViewModel(schemaItem, RemoverColuna, OrganizaPosicao);
                CardsSchema.Add(cardSchema);
                OpcoesPosicao.Add(card.Key);
            }

            AtualizaPosicoes();
        }

        private void RemoverColuna(ConfiguracaoCardSchemaViewModel cardSchema)
        {
            CardsSchema.Remove(cardSchema);
            OpcoesPosicao.Remove(CardsSchema.Count + 1);
            AtualizaPosicoes();
            PreparaParaJson();
        }

        private void AtualizaPosicoes()
        {
            for (int i = 0; i < CardsSchema.Count; i++)
            {
                CardsSchema[i].EstouReposicionando = true;

                CardsSchema[i].OpcoesPosicao = OpcoesPosicao;

                CardsSchema[i].Posicao = i + 1;

                CardsSchema[i].EstouReposicionando = false;
            }
        }

        private void OrganizaPosicao(ConfiguracaoCardSchemaViewModel cardSchema, int posicaoNova)
        {
            int posicaoOriginal = CardsSchema.IndexOf(cardSchema);

            CardsSchema.Move(posicaoOriginal, posicaoNova);

            AtualizaPosicoes();
        }

        private void PreparaParaJson()
        {
            var schemaNovo = new Dictionary<int, SchemaDTO>();

            foreach(var card in CardsSchema)
            {
                var schema = new SchemaDTO(card.NomeColuna, card.Finalidade, card.Tipo, card.Categorico, _nomeModelo);
                schemaNovo.Add(card.Posicao, schema);
            }

            _converter.ConverteJson(schemaNovo);
        }

        [RelayCommand]
        public void NavigateToCarregarDados()
        {
            if (CardsSchema.Count == 0) 
            {
                _dialogService.ShowMessage("Não se pode criar um Schema vazio.", "Schema Vazio");
                return;
            }

            PreparaParaJson();
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
