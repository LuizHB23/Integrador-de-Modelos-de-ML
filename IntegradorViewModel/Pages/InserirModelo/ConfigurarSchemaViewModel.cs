using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PrincipalModelo;

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

        private IConverteJson<SchemaDTO> _converter;
        private string _nomeModelo = string.Empty;

        public ConfigurarSchemaViewModel(INavigationService navigation, IConverteJson<SchemaDTO> converter)
        {
            WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
            {
                 _nomeModelo = m;
            });
            _navigation = navigation;
            _converter = converter;

            NomeColuna = string.Empty;
            Finalidade = string.Empty;
            Tipo = string.Empty;
            Categorico = false;

        }

        [RelayCommand]
        public void AdicinarColuna()
        {
            var schema = new SchemaDTO(_nomeColuna, _finalidade, _tipo, _categorico, _nomeModelo);
            _converter.ConverteJson(schema);
        }

        [RelayCommand]
        public void NavigateToHome() => Navigation.NavigateTo<HomeViewModel>();

        [RelayCommand]
        public void NavigateToCarregarDados() => Navigation.NavigateTo<CarregarDadosViewModel>();
    }
}
