using CommunityToolkit.Mvvm.ComponentModel;
using IntegradorViewModel.PrincipalModelo;
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
    }
}
