using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorViewModel.Context;
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

        [ObservableProperty]
        private string _nomeModelo;

        public ObservableCollection<SchemaItemViewModel> Colunas { get; }

        private IConverteJson<Dictionary<int, SchemaDTO>> _converter;
        private IContext<SchemaItemViewModel> _context;

        public ConfigurarSchemaViewModel(INavigationService navigation, IConverteJson<Dictionary<int, SchemaDTO>> converter, IContext<SchemaItemViewModel> context)
        {
            Navigation = navigation;
            Converter = converter;
            Context = context;

            NomeModelo = string.Empty;
            NomeColuna = string.Empty;
            Finalidade = string.Empty;
            Tipo = string.Empty;
            Categorico = false;
            Colunas = new();
        }

        [RelayCommand]
        public void AdicinarColuna()
        {
            var schema = new SchemaDTO(NomeColuna, Finalidade, Tipo, Categorico);

            if (Colunas.Count == 0)
            {
                var schemaItem = new SchemaItemViewModel(1, schema);
                Colunas.Add(schemaItem);
            }
            else 
            {
                var schemaItem = new SchemaItemViewModel(Colunas.Count + 1, schema);
                Colunas.Add(schemaItem);
            }

            //_converter.ConverteJson(_configuracaoSchema);
        }

        [RelayCommand]
        public void CarregarSchema()
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
