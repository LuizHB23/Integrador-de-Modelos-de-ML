using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PrincipalModelo;

namespace IntegradorViewModel.Pages.InserirModelo
{
    public partial class InserirModeloViewModel : ObservableObject
    {
        [ObservableProperty]
        private NavigationService _navigation;

        public InserirModeloViewModel(NavigationService navigation)
        {
            _navigation = navigation;
        }

        [RelayCommand]
        public void NavigateToHome() => Navigation.NavigateTo<HomeViewModel>();

        [RelayCommand]
        public void NavigateToConfigurarSchema() => Navigation.NavigateTo<ConfigurarSchemaViewModel>();
    }
}
