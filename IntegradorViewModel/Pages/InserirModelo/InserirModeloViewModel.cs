using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InetradorAplicacao.DTO;
using InetradorAplicacao.Gerenciador;
using IntegradorAplicacao.ConversorJSON;
using IntegradorViewModel.Interfaces;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PrincipalModelo;

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

        [ObservableProperty]
        private string _caminhoModelo;

        private readonly IGerenciador<ModeloDTO> _gerenciador;
        private readonly IDialogService _dialogService;
        private readonly IConverteJSON<ModeloDTO> _conversor;

        public InserirModeloViewModel(NavigationService navigation, IGerenciador<ModeloDTO> gerenciador, IDialogService dialogService, IConverteJSON<ModeloDTO> conversor)
        {
            _navigation = navigation;
            _gerenciador = gerenciador;
            _dialogService = dialogService;
            _conversor = conversor;

            CaminhoModelo = string.Empty;
            NomeModelo = string.Empty;
            TipoModelo = string.Empty;
            NomeCaminho = string.Empty;
        }

        [RelayCommand]
        public void BuscaModelo()
        {
            var caminhoArquivo = _dialogService.GetCaminhoArquivo();
            
            if (!string.IsNullOrWhiteSpace(caminhoArquivo))
            {
                string tipoArquivo = Path.GetExtension(caminhoArquivo);

                if (tipoArquivo.Equals(".onnx", StringComparison.OrdinalIgnoreCase))
                {
                    CaminhoModelo = caminhoArquivo;
                    NomeCaminho = Path.GetFileName(caminhoArquivo);
                }
            }
        }

        [RelayCommand]
        public void NavigateToHome() => Navigation.NavigateTo<HomeViewModel>();

        [RelayCommand]
        public void NavigateToConfigurarSchema()
        {
            ConfiguraModelo();
            Navigation.NavigateTo<ConfigurarSchemaViewModel>();
        }

        private void ConfiguraModelo()
        {
            CaminhoModelo = _gerenciador.Salvar(new ModeloDTO(NomeModelo, TipoModelo, CaminhoModelo));

            ModeloDTO modelo = new ModeloDTO(NomeModelo, TipoModelo, CaminhoModelo);

            _conversor.ConverteJSON(modelo);
        }
    }
}
