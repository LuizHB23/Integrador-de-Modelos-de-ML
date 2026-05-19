using CommunityToolkit.Mvvm.ComponentModel;
using IntegradorAplicacao.DTO;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;

namespace IntegradorViewModel.Pages.ConfiguracaoModelo
{
    public partial class TemplateConfiguracaoViewModel : ObservableObject
    {
        public ConfiguracaoModeloViewModel ConfigVM { get; }
        public MetricasModeloViewModel MetricasVM { get; }
        public HistoricoPredicoesViewModel HistoricoVM { get; }

        public TemplateConfiguracaoViewModel(ConfiguracaoModeloViewModel configVM, MetricasModeloViewModel metricasVM, HistoricoPredicoesViewModel historicoVM)
        {
            ConfigVM = configVM;
            MetricasVM = metricasVM;
            HistoricoVM = historicoVM;
        }
    }
}
