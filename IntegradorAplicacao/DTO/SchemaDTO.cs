using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.DTO
{
    public record SchemaDTO(string NomeColuna, string Finalidade, string Tipo, bool Categorico, string NomeModelo);
}
