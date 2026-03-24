using IntegradorDominio.Pipeline.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace IntegradorViewModel.ItensViewModel
{
    public class TransformDataViewItemViewModel
    {
        public ObservableCollection<IStepTransform> ListaProcessos { get; }

        public TransformDataViewItemViewModel(ObservableCollection<IStepTransform> listaProcessos)
        {
            ListaProcessos = listaProcessos;
        }
    }
}
