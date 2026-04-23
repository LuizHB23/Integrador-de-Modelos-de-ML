using IntegradorAplicacao.DTO.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.DTO
{
    public record FuncaoDTO() : IItemNomeModelo, IPipelineExecutor
    {
        public string NomeFuncao { get; set; }
        public List<string> Codigo { get; set; }
        public string NomeModelo { get; set; }
    }
}
