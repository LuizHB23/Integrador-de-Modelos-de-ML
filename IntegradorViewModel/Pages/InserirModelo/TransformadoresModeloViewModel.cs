using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorAplicacao.Infraestrutura.Gerenciador;
using IntegradorDominio.Models.Configuracao;
using IntegradorViewModel.ControleUsuario.ConfiguracaoCard;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using IntegradorViewModel.Shared.Manager.GerenciadorCards;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace IntegradorViewModel.Pages.InserirModelo
{
    public partial class TransformadoresModeloViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _nomeTransformador;

        [ObservableProperty]
        private string _caminhoTransformador;

        [ObservableProperty]
        private INavigationService _navigation;

        public ObservableCollection<int> OpcoesPosicao;
        public ObservableCollection<ConfiguracaoCardTransformadorViewModel> CardsTransformador { get; }

        private readonly CardsTransformadoresModeloManager _cardsManager;
        private string _caminhoProvisorio;
        private string _nomeModelo;

        private readonly IConversorJson _conversor;
        private readonly IGerenciador<TransformadorDTO> _gerenciador;
        private readonly IPathProvider _provider;
        private readonly IDialogService _dialogService;

        public TransformadoresModeloViewModel(INavigationService navigation, IDialogService dialogService, IConversorJson conversor, IContext<ModeloDTO> contextModelo, IPathProvider provider, IGerenciador<TransformadorDTO> gerenciador)
        {
            _conversor = conversor;
            _provider = provider;
            _dialogService = dialogService;
            _gerenciador = gerenciador;
            Navigation = navigation;

            NomeTransformador = string.Empty;
            CaminhoTransformador = string.Empty;
            CardsTransformador = new();
            OpcoesPosicao = new();

            _caminhoProvisorio = string.Empty;
            _nomeModelo = contextModelo.RecebeMensagem().NomeModelo;
            _cardsManager = new(CardsTransformador, OpcoesPosicao);
        }

        [RelayCommand]
        public void AdicionaTransformador()
        {
            if(string.IsNullOrWhiteSpace(NomeTransformador) || string.IsNullOrWhiteSpace(CaminhoTransformador))
            {
                _dialogService.ShowMessage("Não se pode adionar um transformador com Nome ou Caminhos vazios", "Campos Faltantes");
                return;
            }

            var novoCaminho = _gerenciador.Salvar(new TransformadorDTO(NomeTransformador, _caminhoProvisorio) { NomeModelo = _nomeModelo });

            var transformadorItem = new TransformadorItemViewModel(CardsTransformador.Count + 1, NomeTransformador, novoCaminho);

            Debug.WriteLine(transformadorItem.CaminhoTransformador);
            _cardsManager.AdicinarColuna(transformadorItem, RemoverColuna, OrganizaPosicao);
            PreparaParaJson();
        }

        [RelayCommand]
        public void CarregarSchema() => _cardsManager.CarregarTransformador(_dialogService, _conversor);

        private void RemoverColuna(ConfiguracaoCardTransformadorViewModel cardTransformador)
        {
            _cardsManager.RemoverCard(cardTransformador);
            PreparaParaJson();
        }

        private void OrganizaPosicao(ConfiguracaoCardTransformadorViewModel cardTransformador, int posicaoNova)
        {
            _cardsManager.OrganizaPosicao(cardTransformador, posicaoNova);
            PreparaParaJson();
        }

        private void PreparaParaJson() => _cardsManager.PreparaParaJson(_conversor, _nomeModelo);

        [RelayCommand]
        public void CarregarCaminhoTransformadorOnnx()
        {
             var caminhoArquivo = _dialogService.GetCaminhoArquivo();

            if (!string.IsNullOrWhiteSpace(caminhoArquivo))
            {
                string tipoArquivo = Path.GetExtension(caminhoArquivo);

                if (tipoArquivo.Equals(".onnx", StringComparison.OrdinalIgnoreCase))
                {
                    CaminhoTransformador = Path.GetFileName(caminhoArquivo);
                    _caminhoProvisorio = caminhoArquivo;
                }
            }
        }

        [RelayCommand]
        public async Task NavigateToHome()
        {
            var caminhoModelo = _provider.GetCaminhoModeloConfig(_nomeModelo);
            var modelo = await _conversor.CarregarJsonAsync<ModeloConfiguracao>(caminhoModelo);
            modelo.TransformadoresVersao = "1.0";
            await _conversor.ConverteJsonAsync(modelo);

            Navigation.EndFlow();
            Navigation.NavigateTo<HomeViewModel>();
        }

        [RelayCommand]
        public void NavigateToHomeCancelar()
        {
            Navigation.EndFlow();
            Navigation.NavigateTo<HomeViewModel>();
        }
    }
}
