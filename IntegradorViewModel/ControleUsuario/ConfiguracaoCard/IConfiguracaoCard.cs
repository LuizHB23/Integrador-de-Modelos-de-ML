using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace IntegradorViewModel.ControleUsuario.ConfiguracaoCard
{
    public interface IConfiguracaoCard
    {
        int Posicao { get; set; }
        bool EstouReposicionando { get; set; }
        ObservableCollection<int> OpcoesPosicao { get; set; }
    }
}
