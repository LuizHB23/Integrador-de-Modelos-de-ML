using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorViewModel.JanelaModelo;

namespace IntegradorViewModel.Pages.PredicaoModelo
{
    public partial class PreparacaoModeloViewModel : ObservableObject
    {
        [ObservableProperty]
        private NavigationService _navigation;

        public PreparacaoModeloViewModel(NavigationService navigation)
        {
            _navigation = navigation;
        }

        [RelayCommand]
        public void NavigateToResultadoPredicao() => Navigation.NavigateTo<ResultadoPredicaoViewModel>();
    }
}
