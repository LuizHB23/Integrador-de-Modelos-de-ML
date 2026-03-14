using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PredicaoModelo;

namespace IntegradorViewModel.Pages.PrincipalModelo
{
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        private INavigationService _navigation;

        public HomeViewModel(INavigationService navigation)
        {
            _navigation = navigation;
        }

        [RelayCommand]
        public void NavigateToPreparacaoModelo() => Navigation.NavigateTo<PreparacaoModeloViewModel>();
    }
}
