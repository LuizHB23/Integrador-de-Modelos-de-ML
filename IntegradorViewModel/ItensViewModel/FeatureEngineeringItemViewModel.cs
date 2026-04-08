using CommunityToolkit.Mvvm.ComponentModel;
using IntegradorDominio.Attributes;
using IntegradorDominio.InterfacesSteps;
using System.Collections.ObjectModel;

namespace IntegradorViewModel.ItensViewModel
{
    public partial class FeatureEngineeringItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _nomeProcesso;

        [ObservableProperty]
        private IFeature? _funcaoSelecionada;
        public ObservableCollection<IFeature> ListaProcessos { get; }

        private Action<string, List<string>> _devolveListaPropriedades;

        public List<string> PropriedadesSelecionadas { get; private set; } = new();

        public FeatureEngineeringItemViewModel(ObservableCollection<IFeature> listaProcessos, string nomeProcesso, Action<string, List<string>> devolveListaPropriedades)
        {
            ListaProcessos = listaProcessos;
            _nomeProcesso = nomeProcesso;
            _devolveListaPropriedades = devolveListaPropriedades;
        }

        partial void OnFuncaoSelecionadaChanged(IFeature? value)
        {
            if (value == null)
            {
                return;
            }

            var tipo = value.GetType();

            var featureName = tipo
                .GetCustomAttributes(typeof(FeatureNameAttribute), false)
                .Cast<FeatureNameAttribute>()
                .FirstOrDefault()?.Nome;

            // propriedades
            PropriedadesSelecionadas = tipo
                .GetProperties()
                .Where(p => p.CanWrite)
                .Select(p => p.Name)
                .ToList();

            _devolveListaPropriedades(featureName, PropriedadesSelecionadas);
            FuncaoSelecionada = null;
        }
    }
}
