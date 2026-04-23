using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.DTO.Interfaces
{
    public interface IPipelineExecutor
    {
        string NomeFuncao { get; set; }
        List<string> Codigo { get; set; }
        public string NomeModelo { get; set; }
    }
}
