using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.DTO;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using System.Diagnostics;
using System.Text;

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
        private string _pontuacaoDecimal;

        [ObservableProperty]
        private bool _contemCabecalho;

        private readonly IDialogService _dialogService;
        IContext<ArquivoDadosDTO> _contextArquivo;

        public List<string> Delimitadores { get; } = new()
        {
            "Vírgula (,)",
            "Ponto e Vírgula (;)",
            "Tabulação (Tab)"
        };

        public List<string> Codificadores { get; } = new()
        {
            "UTF-8",
            "UTF-16",
            "Windows-1252",
            "ISO-8859-1",
            "ASCII"
        };

        public List<string> PontuacoesDecimal { get; } = new()
        {
            "Ponto (.)",
            "Vírgula (,)"
        };

        public CarregarDadosViewModel(INavigationService navigation, IDialogService dialogService, IContext<ArquivoDadosDTO> contextArquivo)
        {
            _navigation = navigation;
            _dialogService = dialogService;
            _contextArquivo = contextArquivo;

            CaminhoArquivoDados = string.Empty;
            Delimitador = "Vírgula (,)";
            Codificacao = "UTF-8";
            PontuacaoDecimal = "Ponto (.)";
            ContemCabecalho = true;
        }

        [RelayCommand]
        public void CarregarArquivoDados()
        {
            CaminhoArquivoDados = _dialogService.GetCaminhoArquivo()!;
        }

        partial void OnDelimitadorChanged(string value)
        {
            if((value == "Vírgula (,)") && (PontuacaoDecimal == "Vírgula (,)"))
            {
                PontuacaoDecimal = "Ponto (.)";
            }
        }

        partial void OnPontuacaoDecimalChanged(string value)
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

            var arquivoDados = new ArquivoDadosDTO(CaminhoArquivoDados, PegarDelimitador(), PegarEncoding(), PegarPontuacaoDecimal(), ContemCabecalho);
            _contextArquivo.EnviaMensagem(arquivoDados);
            Navigation.NavigateTo<PipelineModeloViewModel>();
        }

        private Encoding PegarEncoding()
        {
            return Codificacao switch
            {
                "UTF-8" => Encoding.UTF8,
                "UTF-16" => Encoding.Unicode,
                "Windows-1252" => Encoding.GetEncoding(1252),
                "ISO-8859-1" => Encoding.GetEncoding("ISO-8859-1"),
                "ASCII" => Encoding.ASCII,
                _ => Encoding.UTF8
            };
        }

        private char PegarDelimitador()
        {
            return Delimitador switch
            {
                "Vírgula (,)" => ',',
                "Ponto e Vírgula (;)" => ';',
                "Tabulação (Tab)" => '\t',
                _ => ','
            };
        }

        private char PegarPontuacaoDecimal()
        {
            return PontuacaoDecimal switch
            {
                "Ponto (.)" => '.',
                "Vírgula (,)" => ',',
                _ => '.'
            };
        }


        [RelayCommand]
        public void NavigateToHome()
        {
            Navigation.EndFlow();
            Navigation.NavigateTo<HomeViewModel>();
        }
    }
}
