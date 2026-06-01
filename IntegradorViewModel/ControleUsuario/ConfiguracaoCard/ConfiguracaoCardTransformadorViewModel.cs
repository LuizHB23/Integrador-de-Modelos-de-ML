using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorViewModel.ItensViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace IntegradorViewModel.ControleUsuario.ConfiguracaoCard
{
    public partial class ConfiguracaoCardTransformadorViewModel : ObservableObject, IConfiguracaoCard
    {
        private readonly Func<ConfiguracaoCardTransformadorViewModel, int, Task> _onTrocarPosicao;
        private readonly Func<ConfiguracaoCardTransformadorViewModel, Task> _onExcluir;
        private TransformadorItemViewModel _transformadorItem;

        [ObservableProperty]
        private ObservableCollection<int> _opcoesPosicao;

        [ObservableProperty]
        private int _posicao;

        [ObservableProperty]
        private string _nomeTransformador;

        [ObservableProperty]
        private string _caminhoTransformador;

        public bool EstouReposicionando { get; set; }
        public string CaminhoProvisorio { get; set; }

        public ConfiguracaoCardTransformadorViewModel(TransformadorItemViewModel transformadorItem, Func<ConfiguracaoCardTransformadorViewModel, Task> funcExcluir, Func<ConfiguracaoCardTransformadorViewModel, int, Task> funcTrocarPosicao)
        {
            _transformadorItem = transformadorItem;
            _onTrocarPosicao = funcTrocarPosicao;
            _onExcluir = funcExcluir;

            OpcoesPosicao = new();
            Posicao = transformadorItem.Posicao;
            NomeTransformador = transformadorItem.NomeTransformador;
            CaminhoTransformador = Path.GetFileName(transformadorItem.CaminhoTransformador);

            CaminhoProvisorio = transformadorItem.CaminhoTransformador;
        }

        partial void OnPosicaoChanged(int value)
        {
            if (!EstouReposicionando && _transformadorItem.Posicao != value)
            {
                _onTrocarPosicao(this, value - 1);
            }

            _transformadorItem.Posicao = value;
        }

        [RelayCommand]
        public void FoiRemovido()
        {
            _onExcluir(this);
        }
    }
}
