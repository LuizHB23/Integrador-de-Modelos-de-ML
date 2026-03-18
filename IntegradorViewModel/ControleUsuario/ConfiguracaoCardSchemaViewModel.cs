using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegradorAplicacao.DTO;
using IntegradorViewModel.Context;
using IntegradorViewModel.ItensViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace IntegradorViewModel.ControleUsuario
{
    public partial class ConfiguracaoCardSchemaViewModel : ObservableObject
    {
        [ObservableProperty]
        private IContext<SchemaItemViewModel> _context;

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

        public ConfiguracaoCardSchemaViewModel(IContext<SchemaItemViewModel> context)
        {
            Context = context;
            NomeColuna = string.Empty;
            Finalidade = string.Empty;
            Tipo = string.Empty;
        }

        [RelayCommand]
        public void FoiRemovido()
        {
            SchemaItem.Mensagem = new SchemaItemViewModel(Posicao, new SchemaDTO(NomeColuna, Finalidade, Tipo, Categorico));
        }

    }
}
