using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.DTO;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Shared.Context;
using System.Diagnostics;

namespace IntegradorViewModel.Pages.PredicaoModelo
{
    public partial class PreparacaoModeloViewModel : ObservableObject
    {
        [ObservableProperty]
        private INavigationService _navigation;

        IContext<ModeloDTO> _context;

        public PreparacaoModeloViewModel(INavigationService navigation, IContext<ModeloDTO> context)
        {
            _navigation = navigation;

            _context = context;
        }

        [RelayCommand]
        public void NavigateToResultadoPredicao() => Navigation.NavigateTo<ResultadoPredicaoViewModel>();
    }
}
