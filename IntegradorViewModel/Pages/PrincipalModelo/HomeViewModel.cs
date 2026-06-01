using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorEnum;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorDominio.Models.Configuracao;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.ConfiguracaoModelo;
using IntegradorViewModel.Pages.PredicaoModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using System.Collections.ObjectModel;

namespace IntegradorViewModel.Pages.PrincipalModelo
{
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        private INavigationService _navigation;

        [ObservableProperty]
        private ObservableCollection<ModeloDTO> _listaModelos;

        private readonly IConversorJson _conversor;
        private readonly IDialogService _dialogService;
        private readonly IContext<ModeloDTO> _context;
        private readonly IPathProvider _provider;

        public HomeViewModel(INavigationService navigation, IConversorJson conversor, IDialogService dialogService, IContext<ModeloDTO> context, IPathProvider provider)
        {
            _dialogService = dialogService;
            _conversor = conversor;
            _context = context;
            _navigation = navigation;
            _provider = provider;
        }

        public async Task InicializarAsync() => ListaModelos = await CarregarModelos();

        public async Task<ObservableCollection<ModeloDTO>> CarregarModelos()
        {
            ListaModelos = new ObservableCollection<ModeloDTO>();

            string caminho = _provider.GetCaminhoPastasMatriz();

            if (!Directory.Exists(caminho))
            {
                return ListaModelos;
            }

            var pastas = Directory.GetDirectories(caminho);

            foreach (var pasta in pastas)
            {
                try
                {
                    var modelo = await _conversor.CarregarJsonAsync<ModeloEmUsoConfiguracao>(pasta);
                    ModeloDTO modeloDTO = new ModeloDTO(modelo.NomeModelo, ParserTipoModelo.TipoModeloParaString(modelo.Tipo), modelo.CaminhoPasta, modelo.Versao);
                    ListaModelos.Add(modeloDTO);
                }
                catch (Exception ex)
                {
                    _dialogService.ShowMessage(ex.Message, "Erro ao Carregar Modelo");
                }
            }

            return ListaModelos;
        }

        [RelayCommand]
        public void NavigateToPreparacaoModelo(ModeloDTO modelo)
        {
            _context.EnviaMensagem(modelo);
            Navigation.NavigateTo<PreparacaoModeloViewModel>();
        }

        [RelayCommand]
        public void NavigateToTemplateConfiguracao(ModeloDTO modelo)
        {
            _context.EnviaMensagem(modelo);
            Navigation.NavigateTo<TemplateConfiguracaoViewModel>();
        }
    }
}
