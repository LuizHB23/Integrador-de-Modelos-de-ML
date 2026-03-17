using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Gerenciador;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao
{
    public static class InjecaoDependencia
    {
        public static IServiceCollection AddIntegradorAplicacaoServices(this IServiceCollection services)
        {
            // Registre aqui tudo o que é "bruto" do sistema
            services.AddTransient<IGerenciador<ModeloDTO>, ModeloGerenciador>();
            services.AddTransient<IConverteJson<ModeloDTO>, ModeloJson>();

            services.AddTransient<IConverteJson<SchemaDTO>, SchemaJson>();

            services.AddSingleton<IPathProvider, PathProvider>();

            return services;
        }
    }
}
