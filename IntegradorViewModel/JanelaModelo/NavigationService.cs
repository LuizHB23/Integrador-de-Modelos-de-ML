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

        private readonly IServiceProvider _rootProvider;
        private IServiceScope? _currentScope;

        public object? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public NavigationService(IServiceProvider rootProvider)
        {
            _rootProvider = rootProvider;
        }

        public void StartFlow()
        {
            _currentScope?.Dispose();
            _currentScope = _rootProvider.CreateScope();
        }

        public void EndFlow()
        {
            _currentScope?.Dispose();
            _currentScope = null;
        }

        public void NavigateTo<TViewModel>() where TViewModel : ObservableObject
        {
            var provider = _currentScope?.ServiceProvider ?? _rootProvider;

            var viewmodel = provider.GetRequiredService<TViewModel>();

            CurrentView = viewmodel;
        }

        //[RelayCommand(CanExecute = nameof(PodeNavegar))]
        //public void NavigateTo<TViewModel>() where TViewModel : ObservableObject
        //{
        //    var viewmodel = _serviceProvider.GetService<TViewModel>();
        //    CurrentView = (ObservableObject)viewmodel!;
        //}

        //private bool PodeNavegar()
        //{
        //    return CurrentView is not ModeloViewModel;
        //}
    }
}

