using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InetradorAplicacao.DTO;
using InetradorAplicacao.Gerenciador;
using IntegradorAplicacao.ConversorJSON;
using IntegradorViewModel.Interfaces;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using Microsoft.VisualBasic;

namespace IntegradorViewModel.Pages.InserirModelo
{
    public partial class InserirModeloViewModel : ObservableObject
    {
        [ObservableProperty]
        private NavigationService _navigation;

        [ObservableProperty]
        private string _nomeModelo;

        [ObservableProperty]
        private string _tipoModelo;

        [ObservableProperty]
        private string _nomeCaminho;

        private string caminhoModelo;

        private readonly IGerenciador<ModeloDTO> _gerenciador;
        private readonly IDialogService _dialogService;
        private readonly IConverteJSON<ModeloDTO> _conversor;

        public InserirModeloViewModel(NavigationService navigation, IGerenciador<ModeloDTO> gerenciador, IDialogService dialogService, IConverteJSON<ModeloDTO> conversor)
        {
            _navigation = navigation;
            _gerenciador = gerenciador;
            _dialogService = dialogService;
            _conversor = conversor;

            caminhoModelo = string.Empty;
            NomeModelo = string.Empty;
            TipoModelo = string.Empty;
            NomeCaminho = string.Empty;
        }

        [RelayCommand]
        public void BuscaModelo()
        {
            var caminhoArquivo = _dialogService.GetCaminhoArquivo();

            if (caminhoArquivo is not null)
            {
                NomeCaminho = Path.GetFileName(caminhoArquivo);
                caminhoModelo = _gerenciador.Salvar(new ModeloDTO(NomeModelo, TipoModelo, caminhoArquivo));
            }
        }

        [RelayCommand]
        public void NavigateToHome() => Navigation.NavigateTo<HomeViewModel>();

        [RelayCommand]
        public void NavigateToConfigurarSchema()
        {
            CriaModeloJSON();
            Navigation.NavigateTo<ConfigurarSchemaViewModel>();
        }

        private void CriaModeloJSON()
        {
            ModeloDTO modelo = new ModeloDTO(NomeModelo, TipoModelo, caminhoModelo);
            _conversor.ConverteJSON(modelo);
        }
    }
}
