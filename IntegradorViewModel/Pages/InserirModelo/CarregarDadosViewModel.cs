using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.DTO;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using System.Diagnostics;

namespace IntegradorViewModel.Pages.InserirModelo
{
    public partial class CarregarDadosViewModel : ObservableObject
    {
        [ObservableProperty]
        private INavigationService _navigation;

        [ObservableProperty]
        private string _caminhoArquivoDados;

        [ObservableProperty]
        private string _delimitador;

        [ObservableProperty]
        private string _codificacao;

        [ObservableProperty]
        private string _decimal;

        [ObservableProperty]
        private bool _contemCabecalho;

        private readonly IDialogService _dialogService;
        IContext<ArquivoDadosDTO> _contextArquivo;

        public CarregarDadosViewModel(INavigationService navigation, IDialogService dialogService, IContext<ArquivoDadosDTO> contextArquivo)
        {
            _navigation = navigation;
            _dialogService = dialogService;
            _contextArquivo = contextArquivo;

            CaminhoArquivoDados = string.Empty;
            Delimitador = "Vírgula (,)";
            Codificacao = "UTF-8";
            Decimal = "Ponto (.)";
            ContemCabecalho = true;
        }

        [RelayCommand]
        public void CarregarArquivoDados()
        {
            CaminhoArquivoDados = _dialogService.GetCaminhoArquivo()!;
        }

        partial void OnDelimitadorChanged(string value)
        {
            if((value == "Vírgula (,)") && (Decimal == "Vírgula (,)"))
            {
                Decimal = "Ponto (.)";
            }
        }

        partial void OnDecimalChanged(string value)
        {
            if((value == "Vírgula (,)") && (Delimitador == "Vírgula (,)"))
            {
                Delimitador = "Ponto e Vírgula (;)";
            }
        }

        [RelayCommand]
        public void NavigateToPipelineModelo()
        {
            if(string.IsNullOrWhiteSpace(CaminhoArquivoDados))
            {
                _dialogService.ShowMessage("Precisa-se de um arquivo prévio", "Schema Vazio");
                //return;
            }


            var arquivoDados = new ArquivoDadosDTO(CaminhoArquivoDados, ',', Codificacao, '.', true);
            _contextArquivo.EnviaMensagem(arquivoDados);
            Navigation.NavigateTo<PipelineModeloViewModel>();
        }

        [RelayCommand]
        public void NavigateToHome()
        {
            Navigation.EndFlow();
            Navigation.NavigateTo<HomeViewModel>();
        }
    }
}
