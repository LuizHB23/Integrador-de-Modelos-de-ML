using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.ArquivosController.Csv.ExportarCsv
{
    public interface ICsvExportador<T>
    {
        void ExportarCsv(T dados);
    }
}
