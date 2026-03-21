using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Gerenciador;
using IntegradorViewModel.Context;
using IntegradorViewModel.Interfaces;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PrincipalModelo;

namespace IntegradorViewModel.Pages.InserirModelo
{
    public partial class InserirModeloViewModel : ObservableObject
    {
        [ObservableProperty]
        private INavigationService _navigation;

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
        private readonly IConverteJson<ModeloDTO> _conversor;
        private readonly IContext<string> _contextNomeModelo;

        public InserirModeloViewModel(INavigationService navigation, IGerenciador<ModeloDTO> gerenciador, IDialogService dialogService, IConverteJson<ModeloDTO> conversor, IContext<string> contextNomeModelo)
        {
            _navigation = navigation;
            _gerenciador = gerenciador;
            _dialogService = dialogService;
            _conversor = conversor;
            _contextNomeModelo = contextNomeModelo;

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
            //Navigation.NavigateTo<ConfigurarSchemaViewModel>();
            if (string.IsNullOrWhiteSpace(NomeModelo) || string.IsNullOrWhiteSpace(TipoModelo) || string.IsNullOrWhiteSpace(CaminhoModelo))
            {
                _dialogService.ShowMessage("Preencha corretamente os campos", "Campos Faltantes");
                return;
            }

            try
            {
                var nomeModelo = ConfiguraModelo();
                _contextNomeModelo.Mensagem = nomeModelo;
                Navigation.NavigateTo<ConfigurarSchemaViewModel>();
            }
            catch (IOException ex)
            {
                _dialogService.ShowMessage($"Nome Inválido: {ex.Message}", "Erro");
            }
        }

        private string ConfiguraModelo()
        {
            CaminhoModelo = _gerenciador.Salvar(new ModeloDTO(NomeModelo, TipoModelo, CaminhoModelo));

            ModeloDTO modelo = new ModeloDTO(NomeModelo, TipoModelo, CaminhoModelo);
            _conversor.ConverteJson(modelo);

            return modelo.Nome;
        }
    }
}
