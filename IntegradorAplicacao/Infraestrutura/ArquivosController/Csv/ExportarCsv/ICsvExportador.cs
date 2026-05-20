using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.Infraestrutura.ArquivosController.Csv.ExportarCsv
{
    public interface ICsvExportador<T>
    {
        string ExportarCsv(T dados);
    }
}
