using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.DTO;
using IntegradorDominio.DataFrameModel;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using System.Diagnostics;
using System.Text;

namespace IntegradorViewModel.Pages.PredicaoModelo
{
    public partial class PreparacaoModeloViewModel : ObservableObject
    {
        [ObservableProperty]
        private INavigationService _navigation;

        [ObservableProperty]
        private string _caminhoArquivoDados;

        IDialogService _dialogService;
        IContext<ArquivoDadosDTO> _context;


        public PreparacaoModeloViewModel(INavigationService navigation, IDialogService dialogService, IContext<ArquivoDadosDTO> context)
        {
            Navigation = navigation;

            _context = context;
            _dialogService = dialogService;

            CaminhoArquivoDados = string.Empty;
        }

        [RelayCommand]
        public void CarregarArquivoDados()
        {
            CaminhoArquivoDados = _dialogService.GetCaminhoArquivo()!;
        }

        [RelayCommand]
        public void NavigateToResultadoPredicao()
        {
            if(string.IsNullOrWhiteSpace(CaminhoArquivoDados))
            {
                return;
            }

            _context.EnviaMensagem(new ArquivoDadosDTO(CaminhoArquivoDados, ' ', "", ' ', true));
            Navigation.NavigateTo<ResultadoPredicaoViewModel>();
        }
    }
}
