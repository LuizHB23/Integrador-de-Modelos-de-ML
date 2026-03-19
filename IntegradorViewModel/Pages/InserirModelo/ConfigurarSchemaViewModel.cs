using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorViewModel.Context;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using System.Collections.ObjectModel;
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

        private IConverteJson<Dictionary<int, SchemaDTO>> _converter;
        private IContext<string> _contextNomeModelo;

        public ConfigurarSchemaViewModel(INavigationService navigation, IConverteJson<Dictionary<int, SchemaDTO>> converter, IContext<string> contextNomeModelo)
        {
            _converter = converter;
            _contextNomeModelo = contextNomeModelo;
            Navigation = navigation;

            _nomeModelo = _contextNomeModelo.Mensagem;
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
            //var schema = new SchemaDTO(NomeColuna, Finalidade, Tipo, Categorico);

            var schemaItem = new SchemaItemViewModel(CardsSchema.Count + 1, NomeColuna, Finalidade, Tipo, Categorico);
            var cardSchema = new ConfiguracaoCardSchemaViewModel(schemaItem, RemoverColuna);
            CardsSchema.Add(cardSchema);

            AtualizaPosicoes(true);

            //_converter.ConverteJson(_configuracaoSchema);
        }

        [RelayCommand]
        public void CarregarSchema()
        {

        }

        private void RemoverColuna(ConfiguracaoCardSchemaViewModel cardSchema)
        {
            OpcoesPosicao.Remove(cardSchema.Posicao);
            CardsSchema.Remove(cardSchema);
            AtualizaPosicoes(false);
        }

        private void AtualizaPosicoes(bool incrementar)
        {
            int quantidade = CardsSchema.Count;
            if (incrementar)
            {
                OpcoesPosicao.Add(quantidade);
            }
            else
            {
                OpcoesPosicao.Remove(quantidade);
            }

            for (int i = 0; i < CardsSchema.Count; i++)
            {
                CardsSchema[i].OpcoesPosicao = OpcoesPosicao;
                CardsSchema[i].Posicao = i + 1;
            }
        }

        private void OrganizaPosicao()
        {

        }

        [RelayCommand]
        public void NavigateToHome() => Navigation.NavigateTo<HomeViewModel>();

        [RelayCommand]
        public void NavigateToCarregarDados()
        {
            //_converter.ConverteJson(_configuracaoSchema);
            Navigation.NavigateTo<CarregarDadosViewModel>();
        }
    }
}
