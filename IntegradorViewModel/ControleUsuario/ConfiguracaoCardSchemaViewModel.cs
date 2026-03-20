using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.DTO;
using IntegradorViewModel.Context;
using IntegradorViewModel.ItensViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace IntegradorViewModel.ControleUsuario
{
    public partial class ConfiguracaoCardSchemaViewModel : ObservableObject
    {
        private readonly Action<ConfiguracaoCardSchemaViewModel, int> _onTrocarPosicao;
        private readonly Action<ConfiguracaoCardSchemaViewModel> _onExcluir;
        private SchemaItemViewModel _schemaItem;

        [ObservableProperty]
        private ObservableCollection<int> _opcoesPosicao;

        [ObservableProperty]
        private int _posicao;

        [ObservableProperty]
        private string _nomeColuna;

        [ObservableProperty]
        private string _finalidade;

        [ObservableProperty]
        private string _tipo;

        [ObservableProperty]
        private bool _categorico;

        public bool EstouReposicionando { get; set; }

        public ConfiguracaoCardSchemaViewModel(SchemaItemViewModel schemaItem, Action<ConfiguracaoCardSchemaViewModel> actionExcluir, Action<ConfiguracaoCardSchemaViewModel, int> actionTrocarPosicao)
        {
            _onTrocarPosicao = actionTrocarPosicao;
            _onExcluir = actionExcluir;
            _schemaItem = schemaItem;

            OpcoesPosicao = new();
            Posicao = _schemaItem.Posicao;
            NomeColuna = _schemaItem.NomeColuna;
            Finalidade = _schemaItem.Finalidade;
            Tipo = _schemaItem.Tipo;
            Categorico = _schemaItem.Categorico;
        }

        partial void OnPosicaoChanged(int value) 
        {
            if (!EstouReposicionando && _schemaItem.Posicao != value)
            {
                _onTrocarPosicao.Invoke(this, value - 1);
            }

            _schemaItem.Posicao = value;
        }

        partial void OnFinalidadeChanged(string value) => _schemaItem.Finalidade = value;

        partial void OnTipoChanged(string value) => _schemaItem.Tipo = value;

        partial void OnCategoricoChanged(bool value) => _schemaItem.Categorico = value;

        [RelayCommand]
        public void FoiRemovido()
        {
            _onExcluir.Invoke(this);
        }
    }
}
