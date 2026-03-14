using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorViewModel.Pages.PrincipalModelo;
using IntegradorViewModel.Pages.InserirModelo;
using IntegradorViewModel.Pages.PredicaoModelo;
using IntegradorViewModel.Pages.GraficoModelo;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorViewModel.JanelaModelo
{
    public partial class NavigationService : ObservableObject, INavigationService
    {
        private object? _currentView;
        private readonly IServiceProvider _serviceProvider;

        public object? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        //[RelayCommand(CanExecute = nameof(PodeNavegar))]
        public void NavigateTo<TViewModel>() where TViewModel : ObservableObject
        {
            var viewmodel = _serviceProvider.GetService<TViewModel>();
            CurrentView = (ObservableObject)viewmodel!;
        }

        //private bool PodeNavegar()
        //{
        //    return CurrentView is not ModeloViewModel;
        //}
    }
}

