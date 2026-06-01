using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorViewModel.ControleUsuario.ConfiguracaoCard;
using IntegradorViewModel.ItensViewModel;
using System.Collections.ObjectModel;

namespace IntegradorViewModel.ControleUsuario
{
    public partial class ConfiguracaoCardFuncaoViewModel : ObservableObject, IConfiguracaoCard
    {
        private readonly Func<ConfiguracaoCardFuncaoViewModel, int, Task> _onTrocarPosicao;
        private readonly Func<ConfiguracaoCardFuncaoViewModel, Task> _onConfigurarFuncao;
        private readonly Func<ConfiguracaoCardFuncaoViewModel, Task> _onExcluir;
        private FuncaoItemViewModel _funcaoItem;

        [ObservableProperty]
        private ObservableCollection<int> _opcoesPosicao;

        [ObservableProperty]
        private int _posicao;

        [ObservableProperty]
        private string _nomeMetodo;

        public bool EstouReposicionando { get; set; }
        public FuncaoItemViewModel FuncaoItem { get => _funcaoItem; }

        public ConfiguracaoCardFuncaoViewModel(FuncaoItemViewModel funcaoItem, Func<ConfiguracaoCardFuncaoViewModel, Task> funcExcluir, Func<ConfiguracaoCardFuncaoViewModel, int, Task> funcTrocarPosicao, Func<ConfiguracaoCardFuncaoViewModel, Task> functionConfigurarFuncao)
        {
            _onConfigurarFuncao = functionConfigurarFuncao;
            _onTrocarPosicao = funcTrocarPosicao;
            _onExcluir = funcExcluir;
            _funcaoItem = funcaoItem;

            OpcoesPosicao = new();
            Posicao = _funcaoItem.Posicao;
            NomeMetodo = _funcaoItem.NomeFuncao;
        }

        partial void OnPosicaoChanged(int value)
        {
            if (!EstouReposicionando && _funcaoItem.Posicao != value)
            {
                _onTrocarPosicao(this, value - 1);
            }

            _funcaoItem.Posicao = value;
        }

        [RelayCommand]
        public void FoiRemovido()
        {
            _onExcluir(this);
        }

        [RelayCommand]
        public void ConfigurarFuncao()
        {
            _onConfigurarFuncao(this);
        }
    }
}
