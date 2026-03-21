using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorViewModel.JanelaModelo
{
    public interface INavigationService
    {
        object? CurrentView { get; set; }

        void NavigateTo<TViewModel>() where TViewModel : ObservableObject;
        void StartFlow();
        void EndFlow();
    }
}
