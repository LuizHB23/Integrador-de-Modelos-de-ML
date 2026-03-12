using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.GraficoModelo;
using IntegradorViewModel.Pages.InserirModelo;
using IntegradorViewModel.Pages.PredicaoModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace IntegradorView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            var services = new ServiceCollection();

            services.AddSingleton<MainWindowViewModel>();

            services.AddTransient<HomeViewModel>();

            services.AddTransient<ResultadoArquivoModeloViewModel>();
            services.AddTransient<PreparacaoModeloViewModel>();
            services.AddTransient<AjusteModeloViewModel>();

            services.AddTransient<PipelineModeloViewModel>();
            services.AddTransient<InserirModeloViewModel>();
            services.AddTransient<ConfigurarSchemaViewModel>();
            services.AddTransient<CarregarDadosViewModel>();

            services.AddTransient<GraficoModeloViewModel>();

            services.AddSingleton<NavigationService>();

            ServiceProvider = services.BuildServiceProvider();
        }
        public static T GetService<T>() where T : notnull => ((App)Current).ServiceProvider.GetRequiredService<T>();

    }
}

