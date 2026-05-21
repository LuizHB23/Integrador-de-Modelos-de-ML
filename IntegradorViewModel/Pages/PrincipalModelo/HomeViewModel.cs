using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.DTO;
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

        private ConversorJson _conversor;
        private IDialogService _dialogService;
        private IContext<ModeloDTO> _context;

        public HomeViewModel(INavigationService navigation, ConversorJson conversor, IDialogService dialogService, IContext<ModeloDTO> context)
        {
            _dialogService = dialogService;
            _conversor = conversor;
            _context = context;

            _navigation = navigation;
            _listaModelos = CarregarModelos();
        }

        public ObservableCollection<ModeloDTO> CarregarModelos()
        {
            ListaModelos = new ObservableCollection<ModeloDTO>();

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string caminhoFinal = Path.Combine(appDataPath, "Integrador", "Modelos");

            if (!Directory.Exists(caminhoFinal))
            {
                return ListaModelos;
            }

            var pastas = Directory.GetDirectories(caminhoFinal);
            var caminhoJson = string.Empty;

            foreach (var pasta in pastas)
            {
                caminhoJson = Path.Combine(caminhoFinal, pasta, "modelo.json");

                try
                {
                    var modelo = _conversor.CarregarJson<ModeloConfiguracao>(caminhoJson);
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
