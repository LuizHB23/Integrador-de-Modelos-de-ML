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
        private FuncaoItemViewModel _funcaoItem;

        [ObservableProperty]
        private ObservableCollection<int> _opcoesPosicao;

        [ObservableProperty]
        private int _posicao;

        [ObservableProperty]
        private string _funcaoSelecionada;

        public bool EstouReposicionando { get; set; }

        public ConfiguracaoCardFuncaoViewModel(FuncaoItemViewModel funcaoItem, Action<ConfiguracaoCardFuncaoViewModel> actionExcluir, Action<ConfiguracaoCardFuncaoViewModel, int> actionTrocarPosicao)
        {
            _onTrocarPosicao = actionTrocarPosicao;
            _onExcluir = actionExcluir;
            _funcaoItem = funcaoItem;

            FuncaoSelecionada = string.Empty;
            OpcoesPosicao = new();
        }
        partial void OnPosicaoChanged(int value)
        {
            if (!EstouReposicionando && _funcaoItem.Posicao != value)
            {
                _onTrocarPosicao.Invoke(this, value - 1);
            }

            _funcaoItem.Posicao = value;
        }

        [RelayCommand]
        public void FoiRemovido()
        {
            _onExcluir.Invoke(this);
        }
    }
}
