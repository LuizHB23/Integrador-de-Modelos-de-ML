using IntegradorAplicacao.DTO;
using Microsoft.Extensions.DependencyInjection;
using IntegradorAplicacao.Aplicacao.InferenciaAplicacao;
using IntegradorAplicacao.Infraestrutura.Gerenciador;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;

namespace IntegradorAplicacao
{
    public static class InjecaoDependencia
    {
        public static IServiceCollection AddIntegradorAplicacaoServices(this IServiceCollection services)
        {
            // Registre aqui tudo o que é "bruto" do sistema
            services.AddTransient<IGerenciador<ModeloDTO>, ModeloGerenciador>();
            services.AddTransient<IGerenciador<TransformadorDTO>, TransformadorGerenciador>();

            services.AddTransient<IConversorJson,ConversorJson>();

            services.AddTransient<Inferencia<FuncaoDTO>>();
            services.AddTransient<Inferencia<SaidaDTO>>();

            services.AddSingleton<IPathProvider, PathProvider>();

            return services;
        }
    }
}
