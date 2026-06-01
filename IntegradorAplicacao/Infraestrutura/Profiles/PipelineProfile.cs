using IntegradorDominio.Models.ModeloEtapas;
using IntegradorAplicacao.DTO;
using AutoMapper;

namespace IntegradorAplicacao.Infraestrutura.Profiles
{
    public class PipelineProfile : Profile
    {
        public PipelineProfile()
        {
            CreateMap<FuncaoDTO, Pipeline>();

            CreateMap<Pipeline, FuncaoDTO>();

            CreateMap<SaidaDTO, Pipeline>();

            CreateMap<Pipeline, SaidaDTO>();
        }
    }
}
