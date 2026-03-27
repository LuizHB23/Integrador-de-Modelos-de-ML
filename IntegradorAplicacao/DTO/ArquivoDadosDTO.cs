using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.DTO
{
    public record ArquivoDadosDTO(string CaminhoArquivoDados, char Delimitador, string Codificacao, char Decimal, bool ContemCabecalho);
}
