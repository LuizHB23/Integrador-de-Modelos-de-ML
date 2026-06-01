using IntegradorAplicacao.DTO;
using Microsoft.Extensions.DependencyInjection;
using IntegradorAplicacao.Aplicacao.InferenciaAplicacao;
using IntegradorAplicacao.Infraestrutura.Gerenciador;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorDominio.Models.Configuracao;
using IntegradorAplicacao.Infraestrutura.Profiles;

namespace IntegradorAplicacao
{
    public static class InjecaoDependencia
    {
        public static IServiceCollection AddIntegradorAplicacaoServices(this IServiceCollection services)
        {
            // Registre aqui tudo o que é "bruto" do sistema
            services.AddTransient<IGerenciador<ModeloDTO>, ModeloGerenciador>();
            services.AddTransient<IGerenciador<TransformadorDTO>, TransformadorGerenciador>();

            services.AddTransient<Inferencia<PipelineTratamentoConfiguracao>>();
            services.AddTransient<Inferencia<PipelineSaidaInferenciaConfiguracao>>();

            services.AddLogging();
            services.AddAutoMapper(cfg => { }, typeof(PipelineProfile), typeof(SchemaProfile));

            services.AddSingleton<IConversorJson,ConversorJson>();
            services.AddSingleton<IPathProvider, PathProvider>();

            return services;
        }
    }
}
