using IntegradorAplicacao;
using IntegradorAplicacao.DTO;
using IntegradorView.InteracoesUI.OpenFileDialog;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Pages.GraficoModelo;
using IntegradorViewModel.Pages.InserirModelo;
using IntegradorViewModel.Pages.PredicaoModelo;
using IntegradorViewModel.Pages.PrincipalModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
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

            services.AddIntegradorAplicacaoServices();

            //Inicio Services ViewModels
            services.AddSingleton<MainWindowViewModel>();

            services.AddTransient<HomeViewModel>();

            services.AddTransient<ResultadoPredicaoViewModel>();
            services.AddTransient<PreparacaoModeloViewModel>();
            services.AddTransient<AjusteModeloViewModel>();

            services.AddTransient<PipelineModeloViewModel>();
            services.AddTransient<InserirModeloViewModel>();
            services.AddTransient<TransformadoresModeloViewModel>();
            services.AddTransient<ConfigurarSchemaViewModel>();
            services.AddTransient<CarregarDadosViewModel>();

            services.AddTransient<GraficoModeloViewModel>();

            //services.AddTransient<ConfiguracaoCardSchemaViewModel>();
            //Fim Services ViewModels

            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddScoped<IContext<ModeloDTO>, NomeModeloContext>();
            services.AddScoped<IContext<ArquivoDadosDTO>, CarregarDadosContext>();

            ServiceProvider = services.BuildServiceProvider();
        }
        public static T GetService<T>() where T : notnull => ((App)Current).ServiceProvider.GetRequiredService<T>();

        protected override void OnStartup(StartupEventArgs e)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            base.OnStartup(e);
        }

    }
}

