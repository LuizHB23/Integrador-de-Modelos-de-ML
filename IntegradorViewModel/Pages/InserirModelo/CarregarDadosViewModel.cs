using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorViewModel.Interfaces;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PrincipalModelo;

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

        public CarregarDadosViewModel(INavigationService navigation, IDialogService dialogService)
        {
            _navigation = navigation;
            _dialogService = dialogService;

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
                return;
            }

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
