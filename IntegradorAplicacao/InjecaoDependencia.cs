using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;
using InetradorAplicacao.Gerenciador;
using IntegradorAplicacao.ConversorJSON;
using InetradorAplicacao.DTO;

namespace IntegradorAplicacao
{
    public static class InjecaoDependencia
    {
        public static IServiceCollection AddIntegradorAplicacaoServices(this IServiceCollection services)
        {
            // Registre aqui tudo o que é "bruto" do sistema
            services.AddTransient<IGerenciador<ModeloDTO>, ModeloGerenciador>();
            services.AddTransient<IConverteJSON<ModeloDTO>, ModeloJSON>();

            return services;
        }
    }
}
