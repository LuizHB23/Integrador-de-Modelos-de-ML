using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.PipelineAplicacao.ExecutorAplicacao;
using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using IntegradorViewModel.Shared.Manager.GerenciadorCards;
using System.Collections.ObjectModel;
using System.Data;

namespace IntegradorViewModel.Pages.InserirModelo
{
    public partial class PipelineModeloViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _proximasFuncoes;

        [ObservableProperty]
        private INavigationService _navigation;

        [ObservableProperty]
        private ConfiguracaoMetodoTextBoxViewModel _textBox;

        [ObservableProperty]
        private DataView _dataPreview;

        public ObservableCollection<int> OpcoesPosicao;
        public ObservableCollection<ConfiguracaoCardFuncaoViewModel> CardsFuncoes { get; }
        public ObservableCollection<FeatureEngineeringItemViewModel> ListaFeatureEngineering { get; }
        public ObservableCollection<TransformDataViewItemViewModel> ListaTransformDataView { get; }

        private  ExecutorFinal? _executor;

        private readonly string _nomeModelo;
        private readonly CardsPipelineModeloManager _cardsManager;

        private readonly IConverteJson<Dictionary<int, FuncaoDTO>> _converter;
        private readonly IDialogService _dialogService;
        private readonly IContext<ArquivoDadosDTO> _contextArquivo;
        private readonly IContext<ModeloDTO> _contextNomeModelo;
        private readonly IPathProvider _provider;

        public PipelineModeloViewModel(INavigationService navigation, IDialogService dialogService, IConverteJson<Dictionary<int, FuncaoDTO>> converter, IContext<ModeloDTO> contextNomeModelo, IContext<ArquivoDadosDTO> contextArquivo, IPathProvider provider)
        {
            _converter = converter;
            _dialogService = dialogService;
            _navigation = navigation;
            _contextNomeModelo = contextNomeModelo;
            _contextArquivo = contextArquivo;
            _provider = provider;

            ListaFeatureEngineering = new();
            ListaTransformDataView = new();
            CarregarListas();

            DataPreview = new();
            CardsFuncoes = new();
            OpcoesPosicao = new();
            TextBox = new ConfiguracaoMetodoTextBoxViewModel(dialogService, _contextArquivo.RecebeMensagem(), AlterouTabela, DataPreview);
            _nomeModelo = _contextNomeModelo.RecebeMensagem().NomeModelo;
            _cardsManager = new(CardsFuncoes, OpcoesPosicao);
        }

        [RelayCommand]
        public void AdicionaFuncao()
        {
            var modeloNomeCorpo = TextBox.MandaCodigoMetodo();

            if ((modeloNomeCorpo is null) || (modeloNomeCorpo.Count == 0))
            {
                return;
            }

            var modeloElementos = modeloNomeCorpo.First();
            var funcaoItem = new FuncaoItemViewModel(CardsFuncoes.Count + 1, modeloElementos.Key, modeloElementos.Value);
            _cardsManager.AdicinarColuna(funcaoItem, RemoverColuna, OrganizaPosicao, ConfigurarFuncao);
            PreparaParaJson();

            try
            {
                ConstroiPipeline();
                TextBox.EsvaziaScript();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Houve um erro no comando: {ex.Message}", "Erro de Comando");
            }
        }

        [RelayCommand]
        public void CarregarSchema()
        {
            _cardsManager.CarregarSchema(_dialogService, _converter, ConfigurarFuncao);
            try
            {
                ConstroiPipeline();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Erro no ao carregar Pipeline: {ex.Message}", "Erro de Comando");
            }
        }

        private void RemoverColuna(ConfiguracaoCardFuncaoViewModel cardSchema)
        {
            _cardsManager.RemoverColuna(cardSchema);
            PreparaParaJson();

            try
            {
                ConstroiPipeline();
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Houve um erro no comando {ex.Message}", "Erro de Comando");
            }
        }

        private void OrganizaPosicao(ConfiguracaoCardFuncaoViewModel cardSchema, int posicaoNova)
        {
            _cardsManager.OrganizaPosicao(cardSchema, posicaoNova);
            PreparaParaJson();
        }

        private void PreparaParaJson() => _cardsManager.PreparaParaJson(_converter, _nomeModelo);

        private void RecebeListaPropriedades(string featureName, List<string> listaPropriedades) => TextBox.EscreveScript(featureName, listaPropriedades);

        public void AlterouTabela(DataView dataView)
        {
            DataPreview = dataView;
        }

        [RelayCommand]
        public void AtualizaFuncao()
        {
            var modeloNomeCorpo = TextBox.MandaCodigoMetodo();

            if (modeloNomeCorpo is null)
            {
                return;
            }

            var caminhoPasta = _provider.GetCaminhoModelo();
            caminhoPasta = Path.Combine(caminhoPasta, _nomeModelo, "pipeline.json");

            var dicionarioFuncoes = _converter.CarregarJson(caminhoPasta);

            foreach (var elemento in dicionarioFuncoes)
            {
                if (elemento.Value.NomeFuncao == modeloNomeCorpo.First().Key)
                {
                    int posicao = elemento.Key;
                    var listaCodigo = modeloNomeCorpo.First().Value;

                    var funcaoDto = new FuncaoDTO(elemento.Value.NomeFuncao, listaCodigo, elemento.Value.NomeModelo);

                    var funcaoReserva = dicionarioFuncoes[posicao];
                    dicionarioFuncoes[posicao] = funcaoDto;
                    _converter.ConverteJson(dicionarioFuncoes);

                    try
                    {
                        ConstroiPipeline();
                        TextBox.EsvaziaScript();
                    }
                    catch (Exception ex)
                    {
                        _dialogService.ShowMessage($"Houve um erro no comando: {ex.Message}", "Erro de Comando");

                        dicionarioFuncoes[posicao] = funcaoReserva;
                        _converter.ConverteJson(dicionarioFuncoes);
                        ConstroiPipeline();
                    }

                    return;
                }
            }

            _dialogService.ShowMessage("Não há método para sobrevescrever");
        }


        public void ConfigurarFuncao(ConfiguracaoCardFuncaoViewModel cardSchema)
        {
            var caminhoPasta = _provider.GetCaminhoModelo();
            caminhoPasta = Path.Combine(caminhoPasta, _nomeModelo, "pipeline.json");

            var dicionarioFuncoes = _converter.CarregarJson(caminhoPasta);
            var codigo = string.Empty;

            foreach (var elemento in dicionarioFuncoes)
            {
                if (elemento.Value.NomeFuncao == cardSchema.NomeMetodo)
                {
                    codigo = $"{cardSchema.NomeMetodo}()" + "\n{";

                    foreach (var linha in elemento.Value.Codigo)
                    {

                        codigo += $"\n{linha}\n";
                    }
                    codigo += "}";
                }
            }

            TextBox.ScriptCodigo = codigo;
        }

        [RelayCommand]
        public void NavigateToHome()
        {
            Navigation.EndFlow();
            Navigation.NavigateTo<HomeViewModel>();
        }

        private void ConstroiPipeline()
        {
            var dataFrame = TextBox.CarregarDados();
            _executor = new(_converter);
            _executor.ConstroiSequenciaMetodoPipeline(Path.Combine(_provider.GetCaminhoModelo(), "pipeline.json"));
            dataFrame = _executor.ExecutarTudo(dataFrame);
            _executor = null;
            TextBox.AtualizaTabela(dataFrame);
        }

        private void CarregarListas()
        {
            var assembly = typeof(IFeature).Assembly;

            var features = assembly.GetTypes()
                .Where(t =>
                    typeof(IFeature).IsAssignableFrom(t) &&
                    !t.IsInterface &&
                    !t.IsAbstract &&
                    t.IsPublic)
                .Select(t => new
                {
                    Tipo = t,
                    Atributo = t.GetCustomAttributes(typeof(FeatureAttribute), false)
                                .Cast<FeatureAttribute>()
                                .FirstOrDefault()
                })
                .Where(x => x.Atributo != null)
                .ToList();

            var grupos = features.GroupBy(x => x.Atributo!.Categoria);

            foreach (var grupo in grupos)
            {
                var instancias = grupo
                    .Select(x => (IFeature)Activator.CreateInstance(x.Tipo)!)
                    .ToList();

                var listaProcessos = new ObservableCollection<IFeature>(instancias);

                var featureItem = new FeatureEngineeringItemViewModel(
                    listaProcessos,
                    grupo.Key,
                    RecebeListaPropriedades
                );

                ListaFeatureEngineering.Add(featureItem);
            }
        }
    }
}
