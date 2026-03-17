using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorViewModel.Context;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using System.Collections.ObjectModel;

namespace IntegradorViewModel.Pages.InserirModelo
{
    public partial class ConfigurarSchemaViewModel : ObservableObject
    {
        [ObservableProperty]
        private INavigationService _navigation;

        [ObservableProperty]
        private string _nomeColuna;

        [ObservableProperty]
        private string _finalidade;

        [ObservableProperty]
        private string _tipo;

        [ObservableProperty]
        private bool _categorico;

        [ObservableProperty]
        private string _nomeModelo;

        public ObservableCollection<SchemaItemViewModel> Colunas { get; }

        private IConverteJson<Dictionary<int, SchemaDTO>> _converter;
        private IContext<string> _context;
        private Dictionary<int, SchemaDTO> _configuracaoSchema;

        public ConfigurarSchemaViewModel(INavigationService navigation, IConverteJson<Dictionary<int, SchemaDTO>> converter, IContext<string> context)
        {
            _navigation = navigation;
            _converter = converter;
            _configuracaoSchema = new Dictionary<int, SchemaDTO>();
            _context = context;

            NomeModelo = string.Empty;
            NomeColuna = string.Empty;
            Finalidade = string.Empty;
            Tipo = string.Empty;
            Categorico = false;
            Colunas = new();
        }

        [RelayCommand]
        public void AdicinarColuna()
        {
            var schema = new SchemaDTO(NomeColuna, Finalidade, Tipo, Categorico, _context.Mensagem);

            if (!_configuracaoSchema.ContainsKey(0))
            {
                var schemaItem = new SchemaItemViewModel(1, schema);
                Colunas.Add(schemaItem);

                _configuracaoSchema.Add(0, schema);
            }
            else 
            {
                var (posicao, _) = _configuracaoSchema.Last();
                _configuracaoSchema.Add(posicao + 1, schema);

                var schemaItem = new SchemaItemViewModel(posicao + 1, schema);
                Colunas.Add(schemaItem);
            }

            //_converter.ConverteJson(_configuracaoSchema);
        }

        [RelayCommand]
        public void CarregarSchema()
        {

        }

        [RelayCommand]
        public void NavigateToHome() => Navigation.NavigateTo<HomeViewModel>();

        [RelayCommand]
        public void NavigateToCarregarDados()
        {
            _converter.ConverteJson(_configuracaoSchema);
            Navigation.NavigateTo<CarregarDadosViewModel>();
        }
    }
}
