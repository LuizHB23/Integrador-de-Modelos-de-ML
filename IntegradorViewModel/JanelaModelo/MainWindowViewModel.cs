using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorViewModel.Pages.GraficoModelo;
using IntegradorViewModel.Pages.InserirModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorViewModel.JanelaModelo
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private NavigationService _navigation;

        public MainWindowViewModel(NavigationService navigationService)
        {
            _navigation = navigationService;

            _navigation.NavigateTo<HomeViewModel>();
        }

        [RelayCommand]
        public void NavigateToHome() => Navigation.NavigateTo<HomeViewModel>();

        [RelayCommand]
        public void NavigateToInserirModelo() => Navigation.NavigateTo<InserirModeloViewModel>();

        [RelayCommand]
        public void NavigateToGraficoModelo() => Navigation.NavigateTo<GraficoModeloViewModel>();
    }
}
