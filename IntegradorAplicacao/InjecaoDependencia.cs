using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;
using IntegradorAplicacao.Gerenciador;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;

namespace IntegradorAplicacao
{
    public static class InjecaoDependencia
    {
        public static IServiceCollection AddIntegradorAplicacaoServices(this IServiceCollection services)
        {
            // Registre aqui tudo o que é "bruto" do sistema
            services.AddTransient<IGerenciador<ModeloDTO>, ModeloGerenciador>();
            services.AddTransient<IConverteJson<ModeloDTO>, ModeloJson>();

            return services;
        }
    }
}
