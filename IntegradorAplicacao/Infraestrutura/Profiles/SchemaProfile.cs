using AutoMapper;
using IntegradorAplicacao.DTO;
using IntegradorDominio.Models.ModeloEtapas;

namespace IntegradorAplicacao.Infraestrutura.Profiles
{
    public class SchemaProfile : Profile
    {
        public SchemaProfile()
        {
            CreateMap<SchemaDTO, Schema>();

            CreateMap<Schema, SchemaDTO>();
        }

    }
}
