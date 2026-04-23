using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using Microsoft.Extensions.DependencyInjection;
using IntegradorAplicacao.InferenciaAplicacao;
using IntegradorAplicacao.Gerenciador;
using IntegradorAplicacao.ConversorJson.CardsJson;

namespace IntegradorAplicacao
{
    public static class InjecaoDependencia
    {
        public static IServiceCollection AddIntegradorAplicacaoServices(this IServiceCollection services)
        {
            // Registre aqui tudo o que é "bruto" do sistema
            services.AddTransient<IGerenciador<ModeloDTO>, ModeloGerenciador>();
            services.AddTransient<IGerenciador<TransformadorDTO>, TransformadorGerenciador>();

            services.AddTransient<IConverteJson<ModeloDTO>, ModeloJson>();
            services.AddTransient<IConverteJson<Dictionary<int, SchemaDTO>>, SchemaJson>();
            services.AddTransient<IConverteJson<Dictionary<int, FuncaoDTO>>, PipelineJson>();
            services.AddTransient<IConverteJson<Dictionary<int, SaidaDTO>>, ResultadoJson>();
            services.AddTransient<IConverteJson<Dictionary<int, TransformadorDTO>>, TransformadorJson>();

            services.AddTransient<Inferencia<FuncaoDTO>>();
            services.AddTransient<Inferencia<SaidaDTO>>();

            services.AddSingleton<IPathProvider, PathProvider>();

            return services;
        }
    }
}
