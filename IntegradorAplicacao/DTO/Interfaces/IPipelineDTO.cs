using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.DTO.Interfaces
{
    public interface IPipelineDTO
    {
        public string NomeModelo { get; set; }
        string NomeFuncao { get; set; }
        List<string> Codigo { get; set; }
    }
}
