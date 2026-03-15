using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PrincipalModelo;

namespace IntegradorViewModel.Pages.InserirModelo
{
    public partial class ConfigurarSchemaViewModel : ObservableObject
    {
        [ObservableProperty]
        private INavigationService _navigation;

        public ConfigurarSchemaViewModel(INavigationService navigation)
        {
            _navigation = navigation;
        }

        [RelayCommand]
        public void NavigateToHome() => Navigation.NavigateTo<HomeViewModel>();

        [RelayCommand]
        public void NavigateToCarregarDados() => Navigation.NavigateTo<CarregarDadosViewModel>();
    }
}
