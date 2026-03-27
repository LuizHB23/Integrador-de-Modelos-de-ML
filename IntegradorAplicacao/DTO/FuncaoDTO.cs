using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.DTO
{
    public record FuncaoDTO(string NomeFuncao, List<string> Codigo, string NomeModelo);
}
