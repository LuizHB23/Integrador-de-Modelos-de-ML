using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace IntegradorViewModel.ControleUsuario
{
    public partial class ConfiguracaoCardSchemaViewModel : ObservableObject
    {
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

        public ConfiguracaoCardSchemaViewModel()
        {
            NomeColuna = string.Empty;
            Finalidade = string.Empty;
            Tipo = string.Empty;
        }

        [RelayCommand]
        public void FoiRemovido()
        {
            Debug.WriteLine("Fui Removido");
        }

    }
}
