using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using IntegradorDominio.Pipeline.InterfacesSteps;

namespace IntegradorViewModel.ItensViewModel
{
    public partial class FeatureEngineeringItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _nomeProcesso;
        public ObservableCollection<IStepFeature> ListaProcessos { get; }

        public FeatureEngineeringItemViewModel(ObservableCollection<IStepFeature> listaProcessos, string nomeProcesso)
        {
            ListaProcessos = listaProcessos;
            _nomeProcesso = nomeProcesso;
        }
    }
}
