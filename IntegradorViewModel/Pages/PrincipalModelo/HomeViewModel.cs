using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorViewModel.JanelaModelo;

namespace IntegradorViewModel.Pages.PrincipalModelo
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly NavigationService _navigation;

        public HomeViewModel(NavigationService navigation)
        {
            _navigation = navigation;
        }

        [RelayCommand]
        private void IrParaSchema()
        {
            _navigation.NavigateTo<HomeViewModel>();
        }
    }
}
