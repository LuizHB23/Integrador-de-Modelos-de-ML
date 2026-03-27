using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace IntegradorViewModel.ItensViewModel
{
    public class TransformDataViewItemViewModel
    {
        public ObservableCollection<ITransform> ListaProcessos { get; }

        public TransformDataViewItemViewModel(ObservableCollection<ITransform> listaProcessos)
        {
            ListaProcessos = listaProcessos;
        }
    }
}
