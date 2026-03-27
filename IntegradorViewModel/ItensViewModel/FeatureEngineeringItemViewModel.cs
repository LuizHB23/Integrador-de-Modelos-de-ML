using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using IntegradorDominio.InterfacesSteps;

namespace IntegradorViewModel.ItensViewModel
{
    public partial class FeatureEngineeringItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _nomeProcesso;
        public ObservableCollection<IFeature> ListaProcessos { get; }

        public FeatureEngineeringItemViewModel(ObservableCollection<IFeature> listaProcessos, string nomeProcesso)
        {
            ListaProcessos = listaProcessos;
            _nomeProcesso = nomeProcesso;
        }
    }
}
