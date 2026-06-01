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
        private ModeloDTO _modelo;

        private readonly IGerenciador<TransformadorDTO> _gerenciador;
        private readonly IDialogService _dialogService;
        private readonly IConversorJson _conversor;

        public TransformadoresModeloViewModel(INavigationService navigation, IDialogService dialogService, IConversorJson conversor, IContext<ModeloDTO> contextModelo, IGerenciador<TransformadorDTO> gerenciador)
        {
            _conversor = conversor;
            _dialogService = dialogService;
            _gerenciador = gerenciador;
            Navigation = navigation;

            NomeTransformador = string.Empty;
            CaminhoTransformador = string.Empty;
            CardsTransformador = new();
            OpcoesPosicao = new();

            _caminhoProvisorio = string.Empty;
            _modelo = contextModelo.RecebeMensagem();
            _cardsManager = new(CardsTransformador, OpcoesPosicao, _modelo);
        }

        [RelayCommand]
        public async Task AdicionaTransformador()
        {
            if(string.IsNullOrWhiteSpace(NomeTransformador) || string.IsNullOrWhiteSpace(CaminhoTransformador))
            {
                _dialogService.ShowMessage("Não se pode adionar um transformador com Nome ou Caminhos vazios", "Campos Faltantes");
                return;
            }

            var novoCaminho = _gerenciador.Salvar(new TransformadorDTO(NomeTransformador, _caminhoProvisorio) { NomeModelo = _modelo.NomeModelo });

            var transformadorItem = new TransformadorItemViewModel(CardsTransformador.Count + 1, NomeTransformador, novoCaminho);

            Debug.WriteLine(transformadorItem.CaminhoTransformador);
            _cardsManager.AdicinarColuna(transformadorItem, RemoverColuna, OrganizaPosicao);
            await PreparaParaJson();
        }

        [RelayCommand]
        public async Task CarregarSchema() => await _cardsManager.CarregarTransformador(_dialogService, _conversor);

        private async Task RemoverColuna(ConfiguracaoCardTransformadorViewModel cardTransformador)
        {
            await _cardsManager.RemoverCard(cardTransformador);
            await PreparaParaJson();
        }

        private async Task OrganizaPosicao(ConfiguracaoCardTransformadorViewModel cardTransformador, int posicaoNova)
        {
            await _cardsManager.OrganizaPosicao(cardTransformador, posicaoNova);
            await PreparaParaJson();
        }

        private async Task PreparaParaJson() => await _cardsManager.PreparaParaJson(_conversor, _modelo.NomeModelo);

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
            var modelo = await _conversor.CarregarJsonAsync<ModeloEmUsoConfiguracao>(_modelo.NomeModelo);
            modelo.TransformadoresVersao = "1.0";
            await _conversor.ConverteJsonAsync(modelo, _modelo.NomeModelo);

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
