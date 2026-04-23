using IntegradorAplicacao.DTO.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.DTO
{
    public record TransformadorDTO(string NomeTransformador, string CaminhoTransformador) : IItemNomeModelo
    {
        public string NomeModelo { get; set; }
    }
}
