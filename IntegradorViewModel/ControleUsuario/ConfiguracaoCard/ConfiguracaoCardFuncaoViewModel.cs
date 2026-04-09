using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.PipelineAplicacao.ParserPipeline;
using IntegradorViewModel.ControleUsuario.ConfiguracaoCard;
using IntegradorViewModel.ItensViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace IntegradorViewModel.ControleUsuario
{
    public partial class ConfiguracaoCardFuncaoViewModel : ObservableObject, IConfiguracaoCard
    {
        private readonly Action<ConfiguracaoCardFuncaoViewModel, int> _onTrocarPosicao;
        private readonly Action<ConfiguracaoCardFuncaoViewModel> _onExcluir;
        private readonly Action<ConfiguracaoCardFuncaoViewModel> _onConfigurarFuncao;
        private FuncaoItemViewModel _funcaoItem;

        [ObservableProperty]
        private ObservableCollection<int> _opcoesPosicao;

        [ObservableProperty]
        private int _posicao;

        [ObservableProperty]
        private string _nomeMetodo;

        public bool EstouReposicionando { get; set; }
        public FuncaoItemViewModel FuncaoItem { get => _funcaoItem; }

        public ConfiguracaoCardFuncaoViewModel(FuncaoItemViewModel funcaoItem, Action<ConfiguracaoCardFuncaoViewModel> actionExcluir, Action<ConfiguracaoCardFuncaoViewModel, int> actionTrocarPosicao, Action<ConfiguracaoCardFuncaoViewModel> actionConfigurarFuncao)
        {
            _onConfigurarFuncao = actionConfigurarFuncao;
            _onTrocarPosicao = actionTrocarPosicao;
            _onExcluir = actionExcluir;
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
