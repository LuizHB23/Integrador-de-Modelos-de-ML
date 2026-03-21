using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.DTO
{
    public record ArquivoDadosDTO(string CaminhoArquivoDados, string Delimitador, string Codificacao, string Decimal, bool ContemCabecalho);
}
