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
        public ObservableCollection<ConfiguracaoCardSchemaViewModel> Colunas { get; }

        private IConverteJson<Dictionary<int, SchemaDTO>> _converter;
        private IContext<string> _context;

        public ConfigurarSchemaViewModel(INavigationService navigation, IConverteJson<Dictionary<int, SchemaDTO>> converter, IContext<string> context)
        {
            _converter = converter;
            _context = context;
            Navigation = navigation;

            _nomeModelo = _context.Mensagem;
            NomeColuna = string.Empty;
            Finalidade = string.Empty;
            Tipo = string.Empty;
            Categorico = false;
            Colunas = new();
        }

        [RelayCommand]
        public void AdicinarColuna()
        {
            //var schema = new SchemaDTO(NomeColuna, Finalidade, Tipo, Categorico);

            var schemaItem = new SchemaItemViewModel(Colunas.Count + 1, NomeColuna, Finalidade, Tipo, Categorico);
            var cardSchema = new ConfiguracaoCardSchemaViewModel(schemaItem, RemoverColuna);
            Colunas.Add(cardSchema);

            //_converter.ConverteJson(_configuracaoSchema);
        }

        [RelayCommand]
        public void CarregarSchema()
        {

        }

        private void RemoverColuna(ConfiguracaoCardSchemaViewModel cardSchema)
        {
            if (cardSchema == null) return;

            Colunas.Remove(cardSchema);

            for (int i = 0; i < Colunas.Count; i++)
            {
                Colunas[i].Posicao = i;
            }

            //AtualizarDadosNoContexto();
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
