using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorViewModel.JanelaModelo;

namespace IntegradorViewModel.Pages.PredicaoModelo
{
    public partial class PreparacaoModeloViewModel : ObservableObject
    {
        [ObservableProperty]
        private INavigationService _navigation;

        public PreparacaoModeloViewModel(INavigationService navigation)
        {
            _navigation = navigation;
        }

        [RelayCommand]
        public void NavigateToResultadoPredicao() => Navigation.NavigateTo<ResultadoPredicaoViewModel>();
    }
}
